using System;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
[Serializable]
public struct Direction : IComponentData
{
    public float3 Dir;
}
