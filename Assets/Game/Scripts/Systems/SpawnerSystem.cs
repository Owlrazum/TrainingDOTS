using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

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

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
	    EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        // Queries for all Spawner components. Uses RefRW because this system wants
        // to read from and write to the component. If the system only needed read-only
        // access, it would use RefRO instead.
        foreach (RefRW<Spawner> spawner in SystemAPI.Query<RefRW<Spawner>>())
        {
	        if (spawner.ValueRO.SpawnRange.x >= spawner.ValueRO.SpawnRange.y)
	        {
		        continue;
	        }
	        
	        // If the next spawn time has passed.
	        while (spawner.ValueRO.NextSpawnTime < SystemAPI.Time.ElapsedTime)
	        {
	            // Spawns a new entity and positions it at the spawner.
	            Entity newEntity = ecb.Instantiate(spawner.ValueRO.Prefab);
	            // LocalPosition.FromPosition returns a Transform initialized with the given position.
	            ecb.SetComponent(newEntity, LocalTransform.FromPosition(spawner.ValueRO.SpawnPosition));
				ecb.AddComponent(newEntity, new Ball() 
					{ Velocity = mRandom.NextFloat3Direction() * spawner.ValueRO.Speed });

		        float3 dirA = new float3( mRandom.NextFloat2Direction(), mRandom.NextFloat() * spawner.ValueRO.Speed);
		        float3 dirB = new float3(mRandom.NextFloat2Direction(), mRandom.NextFloat() * spawner.ValueRO.Speed);
		        float2 dirC = mRandom.NextFloat2Direction() * spawner.ValueRO.Radius;
		        ecb.AddComponent(newEntity, new Spline()
		        { 
			        ControlPoints = new float3x4(spawner.ValueRO.SpawnPosition
						, new float3(dirA.x, 0, dirA.y) * dirA.z 
						, new float3(dirB.x, 0, dirB.y) * dirB.z
						, new float3(dirC.x, 0, dirC.y) + spawner.ValueRO.SpawnPosition), 
			        lerpParam = 0
				});
		        
		        // Resets the next spawn time.
	            spawner.ValueRW.NextSpawnTime = (float)SystemAPI.Time.ElapsedTime + spawner.ValueRO.SpawnRate;
		        spawner.ValueRW.SpawnRange.x++;
	        }
        }
	    ecb.Playback(state.EntityManager);
    }
}
