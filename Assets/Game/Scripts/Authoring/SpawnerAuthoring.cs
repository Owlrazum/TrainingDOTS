using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

class SpawnerAuthoring : MonoBehaviour
{
    public GameObject Prefab;
    public float SpawnRate;
	public float Speed;
	public float Radius = 1;
	public int2 SpawnRange = new int2(0, 10);
}

class SpawnerBaker : Baker<SpawnerAuthoring>
{
    public override void Bake(SpawnerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new Spawner
        {
            // By default, each authoring GameObject turns into an Entity.
            // Given a GameObject (or authoring component), GetEntity looks up the resulting Entity.
            Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
            SpawnPosition = authoring.transform.position,
            NextSpawnTime = 0.0f,
            SpawnRate = authoring.SpawnRate,
	        Speed = authoring.Speed,
	        Radius = authoring.Radius,
	        SpawnRange = authoring.SpawnRange
        });
    }
}	
