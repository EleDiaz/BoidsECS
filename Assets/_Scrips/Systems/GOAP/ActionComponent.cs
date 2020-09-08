using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace _Scrips.Systems.GOAP
{
    [Serializable]
    public struct ActionComponent : IComponentData
    {
        public int Cost;
        public bool Range;
    }

    public struct ConditionComponent : IComponentData {}
    public struct EffectComponent : IComponentData {}

    struct Somthing
    {


    }

    internal enum Amount
    {
    }
}