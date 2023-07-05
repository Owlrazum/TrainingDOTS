using System;
using Unity.Mathematics;
using UnityEngine;

public static class CameraDelegatesContainer
{
    public static Func<float2, Ray> GetScreenRay;
}