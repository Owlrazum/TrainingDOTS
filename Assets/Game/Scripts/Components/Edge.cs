using Unity.Entities;
using Unity.Mathematics;

public struct Edge : IComponentData
{
	public Entity startNode;
    public Entity endNode;
}