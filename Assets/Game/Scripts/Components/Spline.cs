using Unity.Entities;
using Unity.Mathematics;

public struct Spline : IComponentData
{
    public float4x3 ControlPoints;
}

