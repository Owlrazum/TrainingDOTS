using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;

class SelectableAuthoring : MonoBehaviour
{
}

class SelectableBaker : Baker<SelectableAuthoring>
{
    public override void Bake(SelectableAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.WorldSpace);
        AddComponent(entity, new SelectableTag());
        AddComponent(entity, new URPMaterialPropertyBaseColor() { Value = new float4(0, 0, 0, 1)});
    }
}
