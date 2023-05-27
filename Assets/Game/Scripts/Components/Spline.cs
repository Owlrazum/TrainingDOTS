using Unity.Entities;
using Unity.Assertions;
using Unity.Mathematics;
using Unity.Collections;

public struct Spline : IComponentData
{
	public float3x4 ControlPoints;
	public float lerpParam;
	public bool IsPositiveDireciton;
}

/// <summary>
/// Source links: https://en.wikibooks.org/wiki/Cg_Programming/Unity/Hermite_Curves
/// Great video by Freya Holmer: https://www.youtube.com/watch?v=jvPPXbo87ds
/// </summary>
public struct SplineUtil
{
	public static float3 GetCubicBezierPosition(in float3x4 controlPoints, float lerpParam)
	{
		float3x3 first = new float3x3(
			math.lerp(controlPoints[0], controlPoints[1], lerpParam),
			math.lerp(controlPoints[1], controlPoints[2], lerpParam),
			math.lerp(controlPoints[2], controlPoints[3], lerpParam)
		);

		float3x2 second = new float3x2(
			math.lerp(first[0], first[1], lerpParam),
			math.lerp(first[1], first[2], lerpParam)
		);
		
		return math.lerp(second[0], second[1], lerpParam);
	}

	public static float3 GetQuadBezierPosition(in float3x3 controlPoints, float lerpParam)
	{
		// r0 = lerp(p0, p1, t), r1 = lerp(p1, p2, t), r = lerp(r0, r1, t)
		float oneMinus = 1 - lerpParam;
		return oneMinus * oneMinus * controlPoints[0] 
		       + 2.0f * oneMinus * lerpParam * controlPoints[1] 
		       + lerpParam * lerpParam * controlPoints[2];
	}

	/// <summary>
	/// The layout is [p0][v0][v1][p1], where p - position, v - velocity
	/// </summary>
	public static float3 GetHermitePosition(in float3x4 controlPoints, float lerpParam)
	{
		float squared = lerpParam * lerpParam;
		float cubed = squared * lerpParam;
		return (2.0f * cubed - 3.0f * squared + 1.0f) * controlPoints[0] 
			+ (cubed - 2.0f * squared + lerpParam) * controlPoints[1]
			+ (-2.0f * cubed + 3.0f * squared) * controlPoints[3]
			+ (cubed - squared) * controlPoints[2];
	}

	/// <summary>
	/// The lerpParam can exceed one, with each whole number denoting next segment.
	/// The same as Hermite? but with tangents computed automatically by mean.
	/// </summary>
	public static float3 GetCatmullRomPosition(NativeArray<float3> points, float lerpParam, int count = -1)
	{
		count = count < 0 ? points.Length : count;
		int index = (int)math.floor(lerpParam);
		Assert.IsTrue(index >= 0 && index + 1 < count - 1 , "The lerp parameter exceeds count");
		
		float3x4 controlPoints = new(
			points[index]
			, 0
			, 0
			, points[index + 1]);
		
		if (index > 0) controlPoints[2] =  (points[index + 1] - points[index - 1]) / 2;
		else controlPoints[2] = points[index + 1] - points[index];

		if (index < count - 2) controlPoints[3] = (points[index + 2] - points[index]) / 2;
		else controlPoints[3] = points[index + 1] - points[index];
		return GetHermitePosition(controlPoints, math.frac(lerpParam));
	}
}