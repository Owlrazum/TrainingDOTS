using Unity.Entities;
using Unity.Mathematics;

public struct Spawner : IComponentData
{
    public Entity Prefab;
    public float3 SpawnPosition;
    public float NextSpawnTime;
    public float SpawnRate;
	public float Speed;
	public float Radius;
	public int2 SpawnRange;
}
