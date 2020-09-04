using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    [DisableAutoCreation]
    public class MovementTowardsCentre : ComponentSystem
    {
        private EntityQuery _boidsGroup;

        protected override void OnCreate()
        {
            _boidsGroup = GetEntityQuery(
                ComponentType.ReadOnly<BoidGroup>(),
                ComponentType.ReadOnly<Position>(),
                ComponentType.ReadWrite<Direction>());
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

                var cacheSumPosition = new Vector3();
                for (int i = 0; i < directions.Length; i++)
                {
                    cacheSumPosition += positions[i].Pos;
                }

                for (int i = 0; i < directions.Length; i++)
                {
                    Vector3 centre = (cacheSumPosition - positions[i].Pos) / (positions.Length - 1);
                    var centreVector = (centre - positions[i].Pos);
                    var direction = directions[i];
                    direction.Dir += centreVector;
                }
            }
        }
    }
}