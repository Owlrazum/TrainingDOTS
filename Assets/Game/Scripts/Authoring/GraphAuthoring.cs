using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Authoring
{
	public class GraphAuthoring : MonoBehaviour
	{
        public List<int2> Connections;
        public List<float3x2> SplineControls;
        public bool IsHermite;
    }

	class GraphBaker : Baker<GraphAuthoring>
	{
	    public override void Bake(GraphAuthoring authoring)
	    {
	    }
	}	
}