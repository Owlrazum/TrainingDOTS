using System.Text;

using Unity.Mathematics;

using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;

namespace Authoring.Editor
{
	/// <summary>
    /// Limitation, only one instance at a time it seems;
    /// </summary>
    [CustomEditor(typeof(GraphAuthoring))]
    public class SplineEditor : UnityEditor.Editor
    {
        static string mEdgesToShowPrompt = "";

        GraphAuthoring mGraph;
        SerializedProperty mEdgesProperty;
		SerializedProperty mSplineControlsProperty;

        Tool LastTool = Tool.None;

        void OnEnable()
        {
            LastTool = Tools.current;
            Tools.current = Tool.None;
        }

        void OnDisable()
        {
            Tools.current = LastTool;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            mEdgesToShowPrompt = GUILayout.TextField(mEdgesToShowPrompt);

            // if (GUILayout.Button("Test"))
            // {
            //     var m = new float3x4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            //     MathUtil.TestRearrange(m);
            // }
        }

        void OnSceneGUI()
        {
            Tools.current = Tool.None;

            mGraph = target as GraphAuthoring;
            mEdgesProperty = serializedObject.FindProperty("Edges");

            StringBuilder builder = new();
            for (int i = 0; i < mEdgesToShowPrompt.Length; i++)
            {
                if (mEdgesToShowPrompt[i] == ';')
                {
                    if (!int.TryParse(builder.ToString(), out int edgeIndex))
                    {
                        Debug.LogError("Failed to Parse into integer");
                    }
                    builder.Clear();
                    ShowEdge(edgeIndex, true);
                }
                else if (mEdgesToShowPrompt[i] == '|')
                {
                    if (!int.TryParse(builder.ToString(), out int edgeIndex))
                    {
                        Debug.LogError("Failed to Parse into integer");
                    }
                    builder.Clear();
                    ShowEdge(edgeIndex, false);
                }
                else
                {
                    builder.Append(mEdgesToShowPrompt[i]);
                }
            }
        }

        void ShowEdge(int edgeIndex, bool showControls)
        {
            var edgeProp = mEdgesProperty.GetArrayElementAtIndex(edgeIndex);
            var nodesIndices = edgeProp.FindPropertyRelative("NodesIndices");
            var nodeStart = nodesIndices.FindPropertyRelative("x");
            var nodeEnd = nodesIndices.FindPropertyRelative("y");
            if (edgeProp.FindPropertyRelative("PathType").enumValueIndex == 0) // straight
            {
                ShowStraightEdge(new int2(nodeStart.intValue, nodeEnd.intValue), showControls);
            }
            else
            {
                ShowSplineEdge(edgeProp, new int2(nodeStart.intValue, nodeEnd.intValue), showControls);
            }
        }

        void ShowStraightEdge(int2 nodes, bool showControls = true)
        {
            var start = mGraph.transform.GetChild(nodes[0]);
            var end = mGraph.transform.GetChild(nodes[1]);

            float3x2 pos = new float3x2(start.position, end.position);

            if (showControls)
            {
                EditorGUI.BeginChangeCheck();
                pos[0] = Handles.PositionHandle(start.position, Quaternion.identity);
                pos[1] = Handles.PositionHandle(end.position, Quaternion.identity);
            }
            
            Handles.DrawLine(
                pos[0],
                pos[1],
                3);

            if (showControls && EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(start, "change startNode position");
                Undo.RecordObject(end, "change endNode position");

                start.position = pos[0];
                end.position = pos[1];

                serializedObject.ApplyModifiedProperties();
            }
        }

        void ShowSplineEdge(SerializedProperty edgeProp, int2 nodes, bool showControls = true)
        {
            var cs = edgeProp.FindPropertyRelative("StartControlPoint");
			var ce = edgeProp.FindPropertyRelative("EndControlPoint");

            var csx = cs.FindPropertyRelative("x");
            var csy = cs.FindPropertyRelative("y");
            var csz = cs.FindPropertyRelative("z");

            var cex = ce.FindPropertyRelative("x");
            var cey = ce.FindPropertyRelative("y");
            var cez = ce.FindPropertyRelative("z");

            float3x4 spline = new float3x4(
                mGraph.transform.GetChild(nodes[0]).position,
                new float3(csx.floatValue, csy.floatValue, csz.floatValue),
                mGraph.transform.GetChild(nodes[1]).position,
                new float3(cex.floatValue, cey.floatValue, cez.floatValue)
			);

            if (showControls)
            { 
                EditorGUI.BeginChangeCheck();
                spline[1] = Handles.PositionHandle(spline[1], Quaternion.identity);
                spline[3] = Handles.PositionHandle(spline[3], Quaternion.identity);
                spline[0] = Handles.PositionHandle(spline[0], Quaternion.identity);
                spline[2] = Handles.PositionHandle(spline[2], Quaternion.identity);
            }

            float3x4 draw = spline;
            if (edgeProp.FindPropertyRelative("PathType").enumValueFlag == 1) // SplineBeizer
            {
                Handles.DrawBezier(
                    draw[0], draw[2],
                    draw[1], draw[3],
                    Color.white, null, 5);
            }
            else if (edgeProp.FindPropertyRelative("PathType").enumValueFlag == 2) // SplineHermite
            {
                draw[1] = spline[1] - spline[0];
                draw[3] = spline[3] - spline[2];
                draw = SplineUtil.HermiteToBeizer(draw.Rearrange())
                    .Rearrange();
                Handles.DrawBezier(
                    draw[0], draw[3],
                    draw[1], draw[2],
                    Color.white, null, 5);
            }

            if (showControls && EditorGUI.EndChangeCheck())
            {
                var startNode = mGraph.transform.GetChild(nodes[0]);
                var endNode = mGraph.transform.GetChild(nodes[1]);
                Undo.RecordObject(startNode, "change startNode position");
                Undo.RecordObject(endNode, "change endNode position");

                startNode.position = spline[0];
                csx.floatValue = spline[1][0];
				csy.floatValue = spline[1][1];
				csz.floatValue = spline[1][2];

                endNode.position = spline[2];
                cex.floatValue = spline[3][0];
                cey.floatValue = spline[3][1];
                cez.floatValue = spline[3][2];

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}