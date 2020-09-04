using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    [GenerateAuthoringComponent]
    public struct Position : IComponentData
    {
        public Vector3 Pos;
    }
}