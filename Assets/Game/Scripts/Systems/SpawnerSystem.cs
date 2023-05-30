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
    }
}
