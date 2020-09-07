using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    [UpdateAfter(typeof(MatchingSpeed))]
    [UpdateInGroup(typeof(BoidSystem))]
    public class LimitSpeed : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<BoidGroup>()
                .WithoutBurst()
                .ForEach((Entity entity, ref Velocity velocity) =>
                {
                    var boidGroup = EntityManager.GetSharedComponentData<BoidGroup>(entity);
                    velocity.Vel = Mathf.Clamp(velocity.Vel, 0.0f, boidGroup.MaxSpeed);
                }).Run();
        }
    }
}