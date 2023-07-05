using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

#pragma warning disable 282 
partial struct HighlightSystem : ISystem
{
    EntityQuery highlightablesQuery;
#pragma warning restore 282

    public void OnCreate(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        PointerRay ray = SystemAPI.GetSingleton<PointerRay>();
        foreach (var (transform, baseColor, tag) in SystemAPI.Query<
            RefRW<LocalTransform>
            , RefRW<URPMaterialPropertyBaseColor>
            , RefRO<SelectableTag>>())
        {
            float dot = math.dot(ray.direction, math.normalize(transform.ValueRO.Position - ray.origin));
            if (dot > 0.98f)
            {
                baseColor.ValueRW.Value = new float4(1, 1, 1, 1);
            }
            else
            {
                baseColor.ValueRW.Value = new float4(0, 0, 0, 1);
            }
        }
    }
}