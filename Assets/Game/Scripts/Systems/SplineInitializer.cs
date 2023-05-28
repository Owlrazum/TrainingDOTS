using System.Globalization;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateAfter(typeof(SpawnerSystem))]
public partial struct SplineInitializer : ISystem
{
	const float kTolerance = 0.01f;

	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		foreach (var (spline, ball) in SystemAPI.Query<RefRW<Spline>, Ball>())
		{
			float t = spline.ValueRO.LerpParam;
			bool isPositive = spline.ValueRO.IsPositiveDireciton;
			if (math.abs(t - 1) < kTolerance && isPositive)
			{
				spline.ValueRW.IsPositiveDireciton = false;
				float3x4 controlPoints = spline.ValueRO.ControlPoints;
				controlPoints[1] = controlPoints[2];
				controlPoints[2] = spline.ValueRO.ControlPoints[1];

				spline.ValueRW.ControlPoints = controlPoints;
				spline.ValueRW.LeafCount++;
			}
			else if (math.abs(t) < kTolerance && !isPositive)
			{
				spline.ValueRW.IsPositiveDireciton = true;
				float3x4 controlPoints = spline.ValueRO.ControlPoints;
				
				quaternion rotLeft = quaternion.AxisAngle(math.up(), -math.PI / 2 + math.PI / 6 * spline.ValueRO.LeafCount);
				quaternion rotRight = quaternion.AxisAngle(math.up(), math.PI / 2 + math.PI / 6 * spline.ValueRO.LeafCount);
				float3 circle = math.rotate(quaternion.AxisAngle(math.up(), math.PI / 6), math.forward() * ball.Radius);
				controlPoints[1] = math.rotate(rotLeft, math.forward() * ball.Speed);
				controlPoints[2] = math.rotate(rotRight, math.forward() * ball.Speed);
				controlPoints[3] = circle;
				
				spline.ValueRW.ControlPoints = controlPoints;
			}
		}
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState state)
	{

	}
}