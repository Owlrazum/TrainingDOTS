using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(8)]
public struct Node : IBufferElementData
{
	public float3 Position;
}