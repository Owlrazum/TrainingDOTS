using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Authoring
{
	[Serializable]
	public struct Edge
	{
		public int2 StartEnd;
		public float3x4 Spline;
	}
	
	public class GraphAuthoring : MonoBehaviour
	{
		public List<GameObject> Nodes;
		public List<Edge> Edges;
	}

	class GraphBaker : Baker<GraphAuthoring>
	{
	    public override void Bake(GraphAuthoring authoring)
	    {
	    }
	}	
}