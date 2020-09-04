using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public class LimitSpeed : ComponentSystem
    {
        private EntityQuery _boidsGroup;

        protected override void OnCreate()
        {
            _boidsGroup = GetEntityQuery(
                ComponentType.ReadOnly<BoidGroup>(),
                ComponentType.ReadWrite<Velocity>());
        }

        protected override void OnUpdate()
        {
            List<BoidGroup> groups = new List<BoidGroup>();
            EntityManager.GetAllUniqueSharedComponentData(groups);

            foreach (var bGrp in groups)
            {
                _boidsGroup.SetSharedComponentFilter(new BoidGroup {Group = bGrp.Group});
                var velocities = _boidsGroup.ToComponentDataArray<Velocity>(Allocator.Temp);
                for (int i = 0; i < velocities.Length; i++)
                {
                    var velocity = velocities[i];
                    velocity.Vel = Mathf.Clamp(velocity.Vel, 0.0f, bGrp.MaxSpeed);
                }

            }
        }
    }
}