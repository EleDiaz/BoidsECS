using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    [UpdateInGroup(typeof(BoidSystem))]
    [UpdateBefore(typeof(TransformSystemGroup)), UpdateAfter(typeof(LimitSpeed))]
    public class Move : SystemBase
    {
        private List<BoidGroup> _groups;

        protected override void OnCreate()
        {
            _groups = new List<BoidGroup>();
        }

        protected override void OnUpdate()
        {
            EntityManager.GetAllUniqueSharedComponentData(_groups);

            foreach (var bGrp in _groups)
            {
                var time = World.Time.DeltaTime;
                Entities
                    .WithSharedComponentFilter(bGrp)
                    .ForEach((Entity entity, ref Translation translation, ref Rotation rotation, in Direction direction,
                        in Velocity velocity) =>
                    {
                        if (!direction.Dir.Equals(float3.zero))
                        {
                            rotation.Value = math.slerp(rotation.Value,
                                quaternion.LookRotation(direction.Dir, new float3(0, 1, 0)),
                                bGrp.AngularSpeed * time);
                        }

                        translation.Value += math.clamp(math.length(direction.Dir), velocity.Vel, bGrp.MaxSpeed) *
                                             time * math.forward(rotation.Value);
                    }).Run();
            }

            _groups.Clear();
        }
    }
}