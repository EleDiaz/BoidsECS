using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Collider = Unity.Physics.Collider;
using SphereCollider = Unity.Physics.SphereCollider;

namespace DefaultNamespace
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(BoidSystem))]
    [UpdateAfter(typeof(BuildPhysicsWorld)), UpdateBefore(typeof(StepPhysicsWorld)), UpdateBefore(typeof(MovementTowardsCentre))]
    public unsafe class DistancingBasedOnPhysics : SystemBase
    {
        [BurstCompile]
        public struct OverlapCollider : IJob
        {
            [NativeDisableUnsafePtrRestriction] public Collider* Collider;
            public float3 Position;
            public NativeList<ColliderCastHit> ColliderCastHits;
            [ReadOnly] public PhysicsWorld World;

            public void Execute()
            {
                ColliderCastInput colliderCastInput = new ColliderCastInput
                {
                    Collider = Collider,
                    Orientation = quaternion.identity,
                    Start = Position,
                    End = Position
                };

                World.CastCollider(colliderCastInput, ref ColliderCastHits);
            }
        }

        private BuildPhysicsWorld CreatePhysicsWorldSystem;

        private Collider[] _nearObjects;
        private List<BoidGroup> _groups;
        private BlobAssetReference<Collider> _sphereCollider;

        protected override void OnCreate()
        {
            _groups = new List<BoidGroup>();
            CreatePhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _sphereCollider = SphereCollider.Create(new SphereGeometry
            {
                Center = float3.zero,
                Radius = 0.0f,
            });
        }

        protected override void OnUpdate()
        {
            CreatePhysicsWorldSystem.GetOutputDependency().Complete();
            PhysicsWorld world = CreatePhysicsWorldSystem.PhysicsWorld;

            EntityManager.GetAllUniqueSharedComponentData(_groups);

            foreach (var bGrp in _groups)
            {
                ((SphereCollider*) _sphereCollider.GetUnsafePtr())->Geometry = new SphereGeometry { Radius = bGrp.Distancing};
                var collider = _sphereCollider;

                Entities
                    .WithSharedComponentFilter(bGrp)
                    .ForEach((Entity entity, ref Translation position, ref Direction direction) =>
                {
                    var colliderCastHits = new NativeList<ColliderCastHit>(Allocator.TempJob);
                    new OverlapCollider
                    {
                        Collider = (Collider*) collider.GetUnsafePtr(),
                        Position = position.Value,
                        World = world,
                        ColliderCastHits = colliderCastHits
                    }.Schedule().Complete();

                    var c = new float3();
                    for (var i = 0; i < colliderCastHits.Length; i++)
                    {
                        var colliderCastHit = colliderCastHits[i];
                        var outDirection = position.Value - colliderCastHit.Position;
                        var distance = Vector3.Magnitude(outDirection);
                        if (distance < bGrp.Distancing && distance != 0)
                        {
                            // If there are too near make them go different directions
                            c += (bGrp.Distancing / (bGrp.Distancing - distance)) * outDirection;
                        }
                    }

                    colliderCastHits.Dispose();
                    direction.Dir += c;
                }).Run();
            }
            _groups.Clear();
        }
    }
}