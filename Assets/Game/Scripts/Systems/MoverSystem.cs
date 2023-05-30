using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[UpdateAfter(typeof(SpawnerSystem))]
[UpdateAfter(typeof(SplineInitializer))]
partial struct MoverSystem : ISystem
{
	const float kMagnitude = 10;
	const float kSpeed = 1.5f;
    public void OnCreate(ref SystemState state)
    {
		state.RequireForUpdate<Ball>();
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
    }
	
	[BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}