using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateAfter(typeof(MoverSystem))]
public partial struct DestroyerSystem : ISystem
{
	const float kTolerance = 0.01f;
	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		foreach (var spline in SystemAPI.Query<RefRW<Spline>>())
		{
			float t = spline.ValueRO.lerpParam;
			bool isPositive = spline.ValueRO.IsPositiveDireciton;
			if (Math.Abs(t - 1) < kTolerance && isPositive)
			{
				spline.ValueRW.IsPositiveDireciton = false;
			}

			if (MathF.Abs(t) < kTolerance && !isPositive)
			{
				spline.ValueRW.IsPositiveDireciton = true;
			}
		}
	}
}