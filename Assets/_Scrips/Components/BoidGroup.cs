using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{

    [GenerateAuthoringComponent]
    public struct BoidGroup : ISharedComponentData
    {
        public int Group;
        public float AngularSpeed;
        public float MaxSpeed;
        public float Distancing;
    }
}