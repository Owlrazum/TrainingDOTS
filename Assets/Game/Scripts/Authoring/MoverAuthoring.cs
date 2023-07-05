using UnityEngine;
using Unity.Entities;

class MoverAuthoring : MonoBehaviour
{
	public float LerpSpeed = 0.5f;
}

class MoverBaker : Baker<MoverAuthoring>
{
    public override void Bake(MoverAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new Mover
        {
	        lerpSpeed = authoring.LerpSpeed
        });
    }
}	
