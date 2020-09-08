using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace DefaultNamespace
{
    // [DisableAutoCreation]
    [UpdateBefore(typeof(MovementTowardsCentre))]
    [UpdateInGroup(typeof(BoidSystem))]
    public class Distancing : SystemBase
    {
        [BurstCompile]
        private struct Distance : IJob
        {
            public NativeArray<Translation> Positions;
            public NativeArray<Direction> Directions;
            public float Distancing;

            public void Execute()
            {
                for (int i = 0; i < Positions.Length; i++)
                {
                    var c = new float3();
                    for (int j = 0; j < Positions.Length; j++)
                    {
                        var dirNear = Positions[i].Value - Positions[j].Value;
                        var distance = math.length(dirNear);
                        if (distance < Distancing)
                        {
                            c += (Distancing / (Distancing - distance)) * dirNear;
                        }
                    }

                    var direction = Directions[i];
                    direction.Dir += c;
                }
            }
        }

        private EntityQuery _boidsGroup;
        private List<BoidGroup> _groups;

        protected override void OnCreate()
        {
            _groups = new List<BoidGroup>();
            _boidsGroup = GetEntityQuery(
                ComponentType.ReadOnly<BoidGroup>(),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadWrite<Direction>());
        }

        protected override void OnUpdate()
        {
            EntityManager.GetAllUniqueSharedComponentData(_groups);

            foreach (var bGrp in _groups)
            {
                _boidsGroup.AddSharedComponentFilter(bGrp);
                var positions = _boidsGroup.ToComponentDataArray<Translation>(Allocator.Temp);

                Entities
                    .WithSharedComponentFilter(bGrp)
                    .WithReadOnly(positions)
                    .ForEach((ref Direction direction, in Translation position) =>
                    {
                        var c = new float3();
                        for (int j = 0; j < positions.Length; j++)
                        {
                            var dirNear = position.Value - positions[j].Value;
                            var distance = math.length(dirNear);
                            if (distance < bGrp.Distancing)
                            {
                                c += (bGrp.Distancing / (bGrp.Distancing - distance)) * dirNear;
                            }
                        }

                        direction.Dir += c;
                    }).Run();

                positions.Dispose();
                _boidsGroup.ResetFilter();
            }

            _groups.Clear();
        }
    }
}