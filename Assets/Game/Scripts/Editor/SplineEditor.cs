using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Authoring.Editor
{
	[CustomEditor(typeof(GraphAuthoring))]
	public class SplineEditor : UnityEditor.Editor
	{
		GraphAuthoring graph;
		HashSet<int2> constructedEdges;
	
		void OnSceneGUI()
		{
			graph = (GraphAuthoring)target;
			for (int i = 0; i < graph.Edges.Count; i++)
			{
				CheckEdge(i);
			}
		}

		void CheckEdge(int edgeIndex)
		{
			Edge edge = graph.Edges[edgeIndex];
			float3x4 spline = edge.Spline;
			EditorGUI.BeginChangeCheck();
			spline[0] =Handles.PositionHandle(spline[0], Quaternion.identity);
			spline[1] =Handles.PositionHandle(spline[1], Quaternion.identity);
			spline[2] =Handles.PositionHandle(spline[2], Quaternion.identity);
			spline[3] =Handles.PositionHandle(spline[3], Quaternion.identity);
			Handles.DrawBezier(
				spline[0], spline[3],
				spline[1], spline[2],
				Color.red, null, 5);
			if (EditorGUI.EndChangeCheck())
			{
				edge.Spline = spline;
				graph.Edges[edgeIndex] = edge;
			}
		}
	}
}