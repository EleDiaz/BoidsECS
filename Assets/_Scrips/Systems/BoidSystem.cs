using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    [ExecuteAlways]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(BuildPhysicsWorld)), UpdateBefore(typeof(StepPhysicsWorld))]
    public class BoidSystem : ComponentSystemGroup
    {

        protected override void OnUpdate()
        {
            Entities.WithAll<BoidGroup>().ForEach((ref Direction direction, ref Rotation rotation) =>
            {
                direction.Dir = math.forward(rotation.Value);
            });

            base.OnUpdate();
        }
    }
}