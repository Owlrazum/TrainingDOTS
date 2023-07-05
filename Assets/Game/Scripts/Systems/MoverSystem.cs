using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

partial struct InputSystem : ISystem
{
    

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
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