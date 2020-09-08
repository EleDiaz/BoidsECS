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
                Entities
                    .WithAll<BoidGroup>()
                    .ForEach((Entity entity, ref Velocity velocity) =>
                    {
                        velocity.Vel = Mathf.Clamp(velocity.Vel, 0.0f, bGrp.MaxSpeed);
                    }).Run();
            }
            _groups.Clear();
        }
    }
}