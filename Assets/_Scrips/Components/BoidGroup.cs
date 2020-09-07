using System;
using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public struct BoidGroup : ISharedComponentData
    {
        public int Group;
        public float AngularSpeed;
        public float MaxSpeed;
        public float Distancing;
    }
}