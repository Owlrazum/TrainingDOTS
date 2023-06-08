using Unity.Entities;
using Unity.Mathematics;

public struct Edge : IComponentData
{
	public Entity StartNode;
    public Entity EndNode;
}