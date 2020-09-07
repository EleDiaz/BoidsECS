using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;
using SphereCollider = Unity.Physics.SphereCollider;

namespace DefaultNamespace
{
    [UpdateInGroup(typeof(BoidSystem))]
    [UpdateAfter(typeof(BuildPhysicsWorld)), UpdateBefore(typeof(StepPhysicsWorld)), UpdateBefore(typeof(Move))]
    public class AvoidObstacles : SystemBase
    {
        public BuildPhysicsWorld CreatePhysicsWorldSystem { get; set; }

        protected override void OnCreate()
        {
            CreatePhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        }


        protected override void OnUpdate()
        {
            CreatePhysicsWorldSystem.GetOutputDependency().Complete();
            PhysicsWorld world = CreatePhysicsWorldSystem.PhysicsWorld;

            RaycastHit firstHit;

            Entities.WithAll<BoidGroup>().ForEach((ref Direction direction, in Translation translation) =>
            {
                var raycastInput = new RaycastInput
                {
                    Start = translation.Value,
                    End = translation.Value + direction.Dir,
                    Filter = CollisionFilter.Default
                };

                if (world.CastRay(raycastInput, out firstHit))
                {
                    // direction.Dir = math.reflect(direction.Dir, firstHit.SurfaceNormal);
                }
            }).Run();
        }
    }
}