using System;
using System.Collections.Generic;
using UnityEngine;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

namespace Authoring
{
	/// <summary>
    /// The nodes are transforms of this transform
    /// </summary>
	public class GraphAuthoring : MonoBehaviour
	{
        public List<EdgeAuthoring> Edges;
    }

    public enum PathAuthoring // The SplineEditor depends on the order of these elements
    {
        Straight,
        SplineBezier,
        SplineHermite
    }

	[Serializable]
    public class EdgeAuthoring
    {
        public PathAuthoring PathType;
        
		public int2 NodesIndices;

        // in case the pathType is Spline
        public float3 StartControlPoint;
        public float3 EndControlPoint;
    }

    class GraphBaker : Baker<GraphAuthoring>
	{
	    public override void Bake(GraphAuthoring authoring)
	    {
            // var entity = GetEntity(TransformUsageFlags.None);
            NativeArray<Entity> nodes = new(authoring.transform.childCount, Allocator.Temp);
            NativeArray<float3> nodesPos = new(authoring.transform.childCount, Allocator.Temp);
            for (int i = 0; i < authoring.transform.childCount; i++)
            {
                nodes[i] = CreateAdditionalEntity(TransformUsageFlags.None);
                AddComponent(nodes[i], new Node() { position = authoring.transform.GetChild(i).position});
                nodesPos[i] = authoring.transform.GetChild(i).position;
            }

            for (int i = 0; i < authoring.Edges.Count; i++)
            {
                var edgeAuthor = authoring.Edges[i];
                var edge = CreateAdditionalEntity(TransformUsageFlags.None);
                int2 startEnd = edgeAuthor.NodesIndices;
                AddComponent(edge, new Edge() {
                    startNode = nodes[startEnd.x],
                    endNode = nodes[startEnd.y]
                });
                if (edgeAuthor.PathType != PathAuthoring.Straight)
                {
                    if (edgeAuthor.PathType == PathAuthoring.SplineBezier)
                    {
                        AddComponent(edge, new Spline() {
                            controlPoints = new float3x4(
                                nodesPos[startEnd.x]
                                , edgeAuthor.StartControlPoint
                                , nodesPos[startEnd.y]
                                , edgeAuthor.EndControlPoint
                            ).Rearrange()
                        });
                    }
                    else if (edgeAuthor.PathType == PathAuthoring.SplineHermite)
                    {
                        float4x3 controlPoints = new float3x4(
                            nodesPos[startEnd.x]
                            , edgeAuthor.StartControlPoint
                            , nodesPos[startEnd.y]
                            , edgeAuthor.EndControlPoint
                        ).Rearrange();
                        controlPoints = SplineUtil.HermiteToBeizer(controlPoints);
                        AddComponent(edge, new Spline() { controlPoints = controlPoints });
                    }
                }
            }
        }
	}	
}