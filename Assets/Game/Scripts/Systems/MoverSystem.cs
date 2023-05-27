using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[UpdateAfter(typeof(SpawnerSystem))]
partial struct MoverSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
		state.RequireForUpdate<Ball>();
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
	    float lerpSpeed = 0;
	    foreach (var mover in SystemAPI.Query<RefRO<Mover>>())
	    {
		    lerpSpeed = mover.ValueRO.LerpSpeed;
	    }
	    float deltaLerp = SystemAPI.Time.DeltaTime * lerpSpeed;
	    foreach (var (transform, spline) 
	             in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Spline>>())
	    {
		    if (spline.ValueRO.IsPositiveDireciton)
		    {
		        spline.ValueRW.lerpParam = math.min(spline.ValueRO.lerpParam + deltaLerp, 1);
		    }
		    else
		    {
		        spline.ValueRW.lerpParam = math.max(spline.ValueRO.lerpParam - deltaLerp, 0);
		    }
		    
		    float t = spline.ValueRO.lerpParam;
		    float3 newPos = SplineUtil.GetHermitePosition(spline.ValueRO.ControlPoints, t);
		    Debug.DrawLine(transform.ValueRW.Position, newPos, Color.red, 10);
		    transform.ValueRW.Position = newPos;
	    }
    }
	
	[BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}