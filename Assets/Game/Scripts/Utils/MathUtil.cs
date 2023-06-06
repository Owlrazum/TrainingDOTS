using Unity.Mathematics;

public static class MathUtil
{
    public static float3 GetRow(this float4x3 matrix, int rowIndex)
    {
        return new float3(matrix[0][rowIndex], matrix[1][rowIndex], matrix[2][rowIndex]);
    }

    public static float4x3 Rearrange(this float3x4 m)
    {
        return new float4x3(
            m[0][0], m[0][1], m[0][2],
            m[1][0], m[1][1], m[1][2],
            m[2][0], m[2][1], m[2][2],
            m[3][0], m[3][1], m[3][2]
        );
    }

    public static float3x4 Rearrange(this float4x3 m)
    {
        return new float3x4(
            m[0][0], m[0][1], m[0][2], m[0][3],
            m[1][0], m[1][1], m[1][2], m[1][3],
            m[2][0], m[2][1], m[2][2], m[2][3]
        );
    }

    public static void TestRearrange(float3x4 m)
    {
        UnityEngine.Debug.Log($"Before {m}");
        var r = m.Rearrange();
        UnityEngine.Debug.Log($"Rearrange {r}");
        UnityEngine.Debug.Log($"After {r.Rearrange()}");
    }
}