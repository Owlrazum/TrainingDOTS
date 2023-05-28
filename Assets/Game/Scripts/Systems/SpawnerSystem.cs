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
        foreach (RefRW<Spawner> spawner in SystemAPI.Query<RefRW<Spawner>>())
        {
	        if (spawner.ValueRO.SpawnRange.x >= spawner.ValueRO.SpawnRange.y)
	        {
		        continue;
	        }
	        
	        while (spawner.ValueRO.NextSpawnTime < SystemAPI.Time.ElapsedTime)
	        {
	            Entity newEntity = ecb.Instantiate(spawner.ValueRO.Prefab);
	            ecb.SetComponent(newEntity, LocalTransform.FromPosition(spawner.ValueRO.SpawnPosition));
		        float speed = spawner.ValueRO.Speed;
				ecb.AddComponent(newEntity, new Ball() { Speed =  speed, Radius = spawner.ValueRO.Radius});

		        float3 dir = math.forward() * speed;
		        quaternion rotLeft = quaternion.AxisAngle(math.up(), -math.PI / 2);
		        quaternion rotRight = quaternion.AxisAngle(math.up(), math.PI / 2);
		        float3x4 controlPoints = new(spawner.ValueRO.SpawnPosition
			        , math.rotate(rotLeft, dir)
			        , math.rotate(rotRight, dir)
			        , math.forward() * spawner.ValueRO.Radius
				);

		        ecb.AddComponent(newEntity, new Spline()
		        { 
			        ControlPoints = controlPoints
			        , LerpParam = 0
			        , LeafCount = 0
				});
		        
		        // Resets the next spawn time.
	            spawner.ValueRW.NextSpawnTime = (float)SystemAPI.Time.ElapsedTime + spawner.ValueRO.SpawnRate;
		        spawner.ValueRW.SpawnRange.x++;
	        }
        }
	    ecb.Playback(state.EntityManager);
    }
}
