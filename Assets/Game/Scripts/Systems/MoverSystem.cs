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
	    float lerpSpeed = 0;
	    foreach (var mover in SystemAPI.Query<RefRO<Mover>>())
	    {
		    lerpSpeed = mover.ValueRO.LerpSpeed;
	    }
	    // float lerpSpeed = math.sin((float)SystemAPI.Time.ElapsedTime) * 0.5f + 1;
	    float deltaLerp = SystemAPI.Time.DeltaTime * lerpSpeed;
	    foreach (var (transform, spline) 
	             in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Spline>>())
	    {
		    if (spline.ValueRO.IsPositiveDireciton)
		    {
		        spline.ValueRW.LerpParam = math.min(spline.ValueRO.LerpParam + deltaLerp, 1);
		    }
		    else
		    {
		        spline.ValueRW.LerpParam = math.max(spline.ValueRO.LerpParam - deltaLerp, 0);
		    }
		    
		    float t = spline.ValueRO.LerpParam;
		    float3x4 controlPoints = spline.ValueRO.ControlPoints;
		    quaternion rotation = quaternion.AxisAngle(math.up(), (float)SystemAPI.Time.ElapsedTime * kSpeed);
		    controlPoints[1] = math.rotate(rotation, controlPoints[1]);
		    controlPoints[2] = math.rotate(rotation, controlPoints[2]);
		    controlPoints[3] = math.rotate(rotation, controlPoints[3]);
		    float3 newPos = SplineUtil.GetHermitePosition(controlPoints, t);
		    // Debug.DrawLine(transform.ValueRW.Position, newPos, Color.red, 10);

		    float3 circularRotation = new float3(math.cos((float)SystemAPI.Time.ElapsedTime), 0, (float)math.sin(SystemAPI.Time.ElapsedTime));
		    // newPos += circularRotation * kMagnitude;
		    transform.ValueRW.Position = newPos;
	    }
    }
	
	[BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}