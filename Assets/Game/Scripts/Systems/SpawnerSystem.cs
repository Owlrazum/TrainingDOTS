using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile]
public partial struct SpawnerSystem : ISystem
{
	Random mRandom;
	
	public void OnCreate(ref SystemState state)
	{
		mRandom = new Random(1234);
	}

	public void OnDestroy(ref SystemState state)
	{
	}

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
	    EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        // Queries for all Spawner components. Uses RefRW because this system wants
        // to read from and write to the component. If the system only needed read-only
        // access, it would use RefRO instead.
        foreach (RefRW<Spawner> spawner in SystemAPI.Query<RefRW<Spawner>>())
        {
            // If the next spawn time has passed.
        if (spawner.ValueRO.NextSpawnTime < SystemAPI.Time.ElapsedTime)
        {
            // Spawns a new entity and positions it at the spawner.
            Entity newEntity = ecb.Instantiate(spawner.ValueRO.Prefab);
            // LocalPosition.FromPosition returns a Transform initialized with the given position.
            ecb.SetComponent(newEntity, LocalTransform.FromPosition(spawner.ValueRO.SpawnPosition));
			ecb.AddComponent(newEntity, new Ball() 
				{ Velocity = mRandom.NextFloat3Direction() * spawner.ValueRO.Speed });
	        
            // Resets the next spawn time.
            spawner.ValueRW.NextSpawnTime = (float)SystemAPI.Time.ElapsedTime + spawner.ValueRO.SpawnRate;
        }
        }
	    ecb.Playback(state.EntityManager);
    }
}
