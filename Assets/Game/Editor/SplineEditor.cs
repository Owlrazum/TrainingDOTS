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
        SerializedProperty mConnectionsProperty;
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
            mConnectionsProperty = serializedObject.FindProperty("Connections");
            mSplineControlsProperty = serializedObject.FindProperty("SplineControls");

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

                    var edgeProp = mConnectionsProperty.GetArrayElementAtIndex(edgeIndex);
                    var nodeX = edgeProp.FindPropertyRelative("x");
                    var nodeY = edgeProp.FindPropertyRelative("y");
                    ShowEdgeControls(edgeIndex, new int2(nodeX.intValue, nodeY.intValue));
                }
                else
                { 
                    builder.Append(mEdgesToShowPrompt[i]);
                }
            }
        }

        void ShowEdgeControls(int splineControlIndex, int2 nodes)
        {
            var sp = mSplineControlsProperty.GetArrayElementAtIndex(splineControlIndex);
            var cs = sp.FindPropertyRelative("c0");
			var ce = sp.FindPropertyRelative("c1");

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
            EditorGUI.BeginChangeCheck();
            spline[1] = Handles.PositionHandle(spline[1], Quaternion.identity);
            spline[3] = Handles.PositionHandle(spline[3], Quaternion.identity);
            spline[0] = Handles.PositionHandle(spline[0], Quaternion.identity);
            spline[2] = Handles.PositionHandle(spline[2], Quaternion.identity);

            float3x4 draw = spline;
            if (mGraph.IsHermite)
            {
                draw[1] = spline[1] - spline[0];
                draw[3] = spline[3] - spline[2];
                SplineUtil.HermiteToBeizer(draw.Rearrange(), out float4x3 b);
                float3x4 bz = b.Rearrange();
                Handles.DrawBezier(
                    bz[0], bz[3],
                    bz[1], bz[2],
                    Color.red, null, 5);
            }
            else
            {
                Handles.DrawBezier(
                    draw[0], draw[2],
                    draw[1], draw[3],
                    Color.red, null, 5);
            }
            if (EditorGUI.EndChangeCheck())
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