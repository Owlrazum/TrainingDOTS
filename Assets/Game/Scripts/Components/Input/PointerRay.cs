using Unity.Entities;
using Unity.Mathematics;

public struct PointerRay : IComponentData
{
    public float3 origin;
    public float3 direction;
}