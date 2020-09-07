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
        protected override void OnUpdate()
        {
            Entities
                .WithAll<BoidGroup>()
                .WithoutBurst()
                .ForEach((Entity entity, ref Translation translation, ref Rotation rotation, in Direction direction,
                    in Velocity velocity) =>
                {
                    BoidGroup boidGroup = EntityManager.GetSharedComponentData<BoidGroup>(entity);
                    if (!direction.Dir.Equals(float3.zero))
                    {
                        rotation.Value = math.slerp(rotation.Value,
                            quaternion.LookRotation(direction.Dir, new float3(0, 1, 0)),
                            boidGroup.AngularSpeed * Time.DeltaTime);
                    }

                    translation.Value += math.clamp(math.length(direction.Dir), velocity.Vel, boidGroup.MaxSpeed) *
                                         Time.DeltaTime * math.forward(rotation.Value);
                }).Run();
        }
    }
}