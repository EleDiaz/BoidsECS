﻿using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Direction : IComponentData
{
    public Vector3 Dir;
}
