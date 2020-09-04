using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public class Distancing : ComponentSystem
    {
        private EntityQuery _boidsGroup;

        protected override void OnCreate()
        {
            _boidsGroup = GetEntityQuery(
                ComponentType.ReadOnly<BoidGroup>(),
                ComponentType.ReadOnly<Position>(),
                ComponentType.ReadWrite<Direction>(),
                ComponentType.ReadWrite<Velocity>());
        }

        protected override void OnUpdate()
        {
            List<BoidGroup> groups = new List<BoidGroup>();
            EntityManager.GetAllUniqueSharedComponentData(groups);

            foreach (var bGrp in groups)
            {
                _boidsGroup.SetSharedComponentFilter(new BoidGroup {Group = bGrp.Group});
                var positions = _boidsGroup.ToComponentDataArray<Position>(Allocator.Temp);
                var directions = _boidsGroup.ToComponentDataArray<Direction>(Allocator.Temp);

                for (int i = 0; i < positions.Length; i++)
                {
                    var c = new Vector3();
                    for (int j = 0; j < positions.Length; j++)
                    {
                        var dirNear = positions[i].Pos - positions[j].Pos;
                        if (dirNear.sqrMagnitude < bGrp.Distancing * bGrp.Distancing)
                        {
                            c += (dirNear.magnitude) * dirNear;
                        }
                    }

                    var direction = directions[i];
                    direction.Dir += c;
                }
            }
        }
    }
}