using Unity.Mathematics;
using Unity.Collections;
using Unity.Assertions;

/// <summary>
/// Source links: 
/// https://en.wikibooks.org/wiki/Cg_Programming/Unity/Hermite_Curves
/// http://www.joshbarczak.com/blog/?p=730
/// 
/// Great video by Freya Holmer: https://www.youtube.com/watch?v=jvPPXbo87ds
/// 
/// The layout for control points: [p0][v0][p1][v1], where p - position, v - velocity, tangent,
/// float4x3 should be used, it has 3 columns and 4 rows;
/// </summary>
public struct SplineUtil
{
    /// <summary>
    /// c - control points
    /// </summary>
    public static float3 GetCubicBezierPosition(in float4x3 c, float lerpParam)
    {
        // float3x3 first = new float3x3(
        //     math.lerp(controlPoints[0], controlPoints[1], lerpParam),
        //     math.lerp(controlPoints[1], controlPoints[2], lerpParam),
        //     math.lerp(controlPoints[2], controlPoints[3], lerpParam)
        // );

        // float3x2 second = new float3x2(
        //     math.lerp(first[0], first[1], lerpParam),
        //     math.lerp(first[1], first[2], lerpParam)
        // );

        // return math.lerp(second[0], second[1], lerpParam);

        // The reason  to swizzle is because of the way beizer basis is defined:
        // it expects [p0][t0][t1][p1], not [p0][t0][p1][t1]
        float4x3 b = new float4x3(
            c[0][0], c[1][0], c[2][0], 
            c[0][1], c[1][1], c[2][1], 
            c[0][3], c[1][3], c[2][3],
            c[0][2], c[1][2], c[2][2]
        );
        float4 p = GetParameterRow(lerpParam);
        return math.mul(math.mul(p, BezierBasis), b);
    }

    public static float3 GetQuadBezierPosition(in float3x3 controlPoints, float lerpParam)
    {
        // r0 = lerp(p0, p1, t), r1 = lerp(p1, p2, t), r = lerp(r0, r1, t)
        float oneMinus = 1 - lerpParam;
        return oneMinus * oneMinus * controlPoints[0]
               + 2.0f * oneMinus * lerpParam * controlPoints[1]
               + lerpParam * lerpParam * controlPoints[2];
    }

    public static float3 GetHermitePosition(in float4x3 controlPoints, float lerpParam)
    {
        float4 parameters = new float4(1, lerpParam, lerpParam * lerpParam, lerpParam * lerpParam * lerpParam);
        float4 m = math.mul(parameters, HermiteBasis);
        return math.mul(m, controlPoints);
    }

    public static void DrawHermite(in float4x3 controlPoints, float stepSize = 0.1f)
    {
        Assert.IsTrue(stepSize > 0);
        float lerpParam = 0;
        float3 prevPos = controlPoints.GetRow(0);
        int iterations = 0;
        while (lerpParam <= 1 && iterations < 1000)
        {
            lerpParam = math.clamp(lerpParam + stepSize, 0, 1);
            float3 pos = GetHermitePosition(controlPoints, lerpParam);
            UnityEngine.Debug.DrawLine(prevPos, pos, UnityEngine.Color.red);
            prevPos = pos;
            iterations++;
        }
    }

    public static void DrawBezier(in float4x3 controlPoints, float stepSize = 0.1f)
    {
        Assert.IsTrue(stepSize > 0);
        float lerpParam = 0;
        float3 prevPos = controlPoints.GetRow(0);
        int iterations = 0;
        while (lerpParam <= 1 && iterations < 1000)
        {
            lerpParam = math.clamp(lerpParam + stepSize, 0, 1);
            float3 pos = GetCubicBezierPosition(controlPoints, lerpParam);
            UnityEngine.Debug.DrawLine(prevPos, pos, UnityEngine.Color.red);
            prevPos = pos;
            iterations++;
        }
    }

    public static float4x3 HermiteToBeizer(in float4x3 hermite)
    {
        return math.mul(math.mul(InverseBeizer, HermiteBasis), hermite);
    }

    /// <summary>
    /// The lerpParam can exceed one, with each whole number denoting next segment.
    /// The same as Hermite? but with tangents computed automatically by mean.
    /// </summary>
    public static float3 GetCatmullRomPosition(NativeArray<float3> points, float lerpParam, int count = -1)
    {
        count = count < 0 ? points.Length : count;
        int index = (int)math.floor(lerpParam);
        Assert.IsTrue(index >= 0 && index + 1 < count - 1, "The lerp parameter exceeds count");

        float3x4 controlPoints = new(
            points[index]
            , 0
            , points[index + 1]
            , 0);

        if (index > 0) controlPoints[1] = (points[index + 1] - points[index - 1]) / 2;
        else controlPoints[1] = points[index + 1] - points[index];

        if (index < count - 2) controlPoints[2] = (points[index + 2] - points[index]) / 2;
        else controlPoints[2] = points[index + 1] - points[index];
        return GetHermitePosition(MathUtil.Rearrange(controlPoints), math.frac(lerpParam));
    }

    #region Helpers
    /// <summary>
    /// http://www.joshbarczak.com/blog/?p=730
    /// </summary>
    static float4x4 InverseBeizer = new float4x4(
        1,      0,      0, 0,
        1, 1 / 3f,      0, 0,
        1, 2 / 3f, 1 / 3f, 0,
        1,      1,      1, 1
    );

    static float4x4 HermiteBasis = new float4x4(
         1,  0,  0,  0,
         0,  1,  0,  0,
        -3, -2,  3, -1,
         2,  1, -2,  1
    );
    static float4x4 HermiteToBeizerConversion = math.mul(math.inverse(BezierBasis), HermiteBasis);
    

    static float4x4 BezierBasis = new float4x4(
         1,  0,  0, 0,
        -3,  3,  0, 0,
         3, -6,  3, 0,
        -1,  3, -3, 1
    );

    static float4x4 BSplineBasis = 1 / 6f * new float4x4(
         1,  3, -3, 1,
         3, -6,  3, 0,
        -3,  0,  3, 0,
         1,  4,  1, 0
    );
    static float4x4 BSplineToBeizerConversion = math.mul(InverseBeizer, BSplineBasis);

    static float4 GetParameterRow(float lerpParam)
    {
        float q = lerpParam * lerpParam;
        return new float4(1, lerpParam, q, q * lerpParam);
    }
    #endregion
}