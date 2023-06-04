using System;
using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Authoring.Editor
{
	[CustomEditor(typeof(GraphAuthoring))]
	public class SplineEditor : UnityEditor.Editor
	{
		public StyleSheet Style;

		SerializedProperty mNodesProperty;
		SerializedProperty mEdgesProperty;
		GraphAuthoring mGraph;
		HashSet<int2> mConstructedEdges;

		List<IDisposable> mRegisteredTokens = new List<IDisposable>();
		
		public override VisualElement CreateInspectorGUI()
		{
			mNodesProperty = serializedObject.FindProperty("Nodes");
			mEdgesProperty = serializedObject.FindProperty("Edges");

			// mGraph = target as GraphAuthoring;
			Assert.IsTrue(mGraph != null);
			
			VisualElement container = new VisualElement();
			container.styleSheets.Add(Style);
			container.AddToClassList("spline-editor");

			ListView nodes = new ListView();
			ListView edges = new ListView();
			
			nodes.AddToClassList("spline-editor__nodes");
			edges.AddToClassList("spline-editor__edges");
			
			container.Add(nodes);
			container.Add(edges);

			Func<VisualElement> makeNode = RenderNodeUI;
			Action<VisualElement, int> bindNode = BindNodeUI;
			
			Func<VisualElement> makeEdge = RenderEdgeUI;
			Action<VisualElement, int> bindEdge = BindEdgeUI;
			
			nodes.makeItem = makeNode;
			nodes.bindItem = bindNode;
			nodes.itemsSource = mNodesProperty.array mGraph.Nodes;
			nodes.selectionType = SelectionType.Single;
			nodes.showAddRemoveFooter = true;

			edges.makeItem = makeEdge;
			edges.bindItem = bindEdge;
			edges.itemsSource = mGraph.Edges;
			edges.selectionType = SelectionType.Single;
			edges.showAddRemoveFooter = true;

			// Callback invoked when the user double clicks an item
			edges.itemsChosen += Debug.Log;

			// Callback invoked when the user changes the selection inside the ListView
			edges.selectionChanged += Debug.Log;
			return container;
		}
		
		VisualElement RenderNodeUI()
		{
			ObjectField field = new ObjectField{label = "Node", objectType = typeof(GameObject)};
			return field;
		}
		
		void BindNodeUI(VisualElement v, int index)
		{
			ObjectField field = v as ObjectField;
			field.value = mGraph.Nodes[index];

			EventCallback<ChangeEvent<UnityEngine.Object>> lambda = (evt) => {
				mGraph.Nodes[index] = evt.newValue as GameObject;
			};

			RegisterCallback(field, lambda);
		}

		VisualElement RenderEdgeUI()
		{
			Vector2IntField startEndUI = new();
			return startEndUI;
		}

		void BindEdgeUI(VisualElement v, int index)
		{
			var startEndUI = v.Q<Vector2IntField>();
			int2 value = mGraph.Edges[index].StartEnd;
			startEndUI.value = new Vector2Int(value.x, value.y);

			EventCallback<ChangeEvent<Vector2Int>> lambda = (evt) => {
				Edge edge = mGraph.Edges[index];
				edge.StartEnd.x = evt.newValue.x;
				edge.StartEnd.y = evt.newValue.y;
				mGraph.Edges[index] = edge;
			};

			RegisterCallback(startEndUI, lambda);
		}

		void OnSceneGUI()
		{
			// graph = (GraphAuthoring)target;
			// for (int i = 0; i < graph.Edges.Count; i++)
			// {
			// 	CheckEdge(i);
			// }
		}

		void CheckEdge(int edgeIndex)
		{
			Edge edge = mGraph.Edges[edgeIndex];
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
				mGraph.Edges[edgeIndex] = edge;
			}
		}

		class Vector2IntRegistratinToken : IDisposable
		{
			public Vector2IntField Field;
			public EventCallback<ChangeEvent<Vector2Int>> ChangeEvent;
			public void Dispose() => Field.UnregisterValueChangedCallback(ChangeEvent);
		}

		class ObjectFieldRegistratinToken : IDisposable
		{
			public ObjectField Field;
			public EventCallback<ChangeEvent<UnityEngine.Object>> ChangeEvent;
			public void Dispose() => Field.UnregisterValueChangedCallback(ChangeEvent);
		}

		void RegisterCallback(Vector2IntField field, EventCallback<ChangeEvent<Vector2Int>> changeEvent)
		{
			field.RegisterValueChangedCallback(changeEvent);
			mRegisteredTokens.Add(new Vector2IntRegistratinToken { Field = field, ChangeEvent = changeEvent });
		}

		void RegisterCallback(ObjectField field, EventCallback<ChangeEvent<UnityEngine.Object>> changeEvent)
		{
			field.RegisterValueChangedCallback(changeEvent);
			mRegisteredTokens.Add(new ObjectFieldRegistratinToken { Field = field, ChangeEvent = changeEvent });
		}
		
		public override void DiscardChanges()
		{
			base.DiscardChanges();
			mRegisteredTokens.ForEach(token => token.Dispose());
			mRegisteredTokens.Clear();
		}
	}
}