using Unity.Entities;
using Unity.Mathematics;

public struct Spline : IComponentData
{
	public float3x4 ControlPoints;
}

