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
                    var edgeX = edgeProp.FindPropertyRelative("x");
                    var edgeY = edgeProp.FindPropertyRelative("y");
                    ShowEdgeControls(edgeX.intValue, edgeY.intValue);
                }
                builder.Append(mEdgesToShowPrompt[i]);
            }
        }

        void ShowEdgeControls(int start, int end)
        {
            var cs = mSplineControlsProperty.GetArrayElementAtIndex(start);
			var ce = mSplineControlsProperty.GetArrayElementAtIndex(end);

            var csx = cs.FindPropertyRelative("x");
            var csy = cs.FindPropertyRelative("y");
            var csz = cs.FindPropertyRelative("z");

            var cex = ce.FindPropertyRelative("x");
            var cey = ce.FindPropertyRelative("y");
            var cez = ce.FindPropertyRelative("z");

            float3x4 spline = new float3x4(
                mGraph.transform.GetChild(start).position,
                new float3(csx.floatValue, csy.floatValue, csz.floatValue),
                new float3(cex.floatValue, cey.floatValue, cez.floatValue),
                mGraph.transform.GetChild(end).position
			);
            EditorGUI.BeginChangeCheck();
            spline[0] = Handles.PositionHandle(spline[0], Quaternion.identity);
            spline[1] = Handles.PositionHandle(spline[1], Quaternion.identity);
            spline[2] = Handles.PositionHandle(spline[2], Quaternion.identity);
            spline[3] = Handles.PositionHandle(spline[3], Quaternion.identity);
            Handles.DrawBezier(
                spline[0], spline[3],
                spline[1], spline[2],
                Color.red, null, 5);
            if (EditorGUI.EndChangeCheck())
            {
                mGraph.transform.GetChild(start).position = spline[0];

                csx.floatValue = spline[1][0];
				csy.floatValue = spline[1][1];
				csz.floatValue = spline[1][2];

                cex.floatValue = spline[2][0];
                cey.floatValue = spline[2][1];
                cez.floatValue = spline[2][2];

                mGraph.transform.GetChild(end).position = spline[3];

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}