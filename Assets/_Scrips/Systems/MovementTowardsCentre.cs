using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    [UpdateInGroup(typeof(BoidSystem))]
    [UpdateBefore(typeof(LimitSpeed)), UpdateBefore(typeof(MatchingSpeed))]
    public class MovementTowardsCentre : SystemBase
    {
        // [BurstCompile]
        private struct SumOfPositions : IJob
        {
            public NativeArray<Translation> Positions;
            public NativeReference<float3> SumResult;

            public void Execute()
            {
                for (int i = 0; i < Positions.Length; i++)
                {
                    SumResult.Value += Positions[i].Value;
                }

            }
        }

        private EntityQuery _boidsGroup;
        private NativeArray<Translation> _positions;
        private List<BoidGroup> _groups;

        protected override void OnCreate()
        {
            _groups = new List<BoidGroup>();
            _boidsGroup = GetEntityQuery(
                ComponentType.ReadOnly<BoidGroup>(),
                ComponentType.ReadOnly<Translation>());
        }

        protected override void OnUpdate()
        {
            EntityManager.GetAllUniqueSharedComponentData(_groups);

            foreach (var bGrp in _groups)
            {
                _boidsGroup.AddSharedComponentFilter(bGrp);
                _positions = _boidsGroup.ToComponentDataArray<Translation>(Allocator.TempJob);
                var amount = _positions.Length;

                var sumPos = new NativeReference<float3>(Allocator.TempJob);
                var sumOfPositions = new SumOfPositions
                {
                    Positions = _positions,
                    SumResult = sumPos
                };
                var finishSums = sumOfPositions.Schedule(Dependency);

                var jobHandle = Entities
                    .WithSharedComponentFilter(bGrp)
                    .ForEach(
                        (ref Direction direction, in Translation position) =>
                        {
                            float3 centre = (sumPos.Value - position.Value) / (amount - 1);
                            var centreVector = centre - position.Value;
                            direction.Dir += centreVector;
                        }).Schedule(JobHandle.CombineDependencies(finishSums, Dependency));

                _positions.Dispose(jobHandle);
                sumPos.Dispose(jobHandle);
                jobHandle.Complete();
                _boidsGroup.ResetFilter();
            }

            _groups.Clear();
        }
    }
}