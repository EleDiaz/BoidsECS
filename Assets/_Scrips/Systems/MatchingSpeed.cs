using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace DefaultNamespace
{
    [UpdateInGroup(typeof(BoidSystem))]
    [UpdateBefore(typeof(LimitSpeed))]
    public class MatchingSpeed : SystemBase
    {
        [BurstCompile]
        private struct SumOfVelocities : IJob
        {
            public NativeArray<Velocity> Velocities;
            public float SumResult;

            public void Execute()
            {
                for (int i = 0; i < Velocities.Length; i++)
                {
                    SumResult += Velocities[i].Vel;
                }
            }
        }

        private EntityQuery _boidsGroup;
        private NativeArray<Velocity> _velocities;
        private List<BoidGroup> _groups;

        protected override void OnCreate()
        {
            _groups = new List<BoidGroup>();
            _boidsGroup = GetEntityQuery(
                ComponentType.ReadOnly<BoidGroup>(),
                ComponentType.ReadWrite<Velocity>());
        }

        protected override void OnUpdate()
        {
            EntityManager.GetAllUniqueSharedComponentData(_groups);

            foreach (var bGrp in _groups)
            {
                _boidsGroup.AddSharedComponentFilter(bGrp);
                _velocities = _boidsGroup.ToComponentDataArray<Velocity>(Allocator.TempJob);
                var amount = _velocities.Length;

                var sumVel = new SumOfVelocities
                {
                    Velocities = _velocities
                };
                sumVel.Schedule().Complete();

                Entities
                    .WithSharedComponentFilter(bGrp)
                    .ForEach((ref Velocity velocity) =>
                    {
                        var sum = sumVel.SumResult - velocity.Vel; // Remove its own velocity to get precise average
                        velocity.Vel = sum / (amount - 1);
                    }).Run();

                _velocities.Dispose();
                _boidsGroup.ResetFilter();
            }
            _groups.Clear();
        }
    }
}