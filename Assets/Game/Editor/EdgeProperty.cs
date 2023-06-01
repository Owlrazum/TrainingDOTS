using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Authoring.Editor
{
	[CustomPropertyDrawer(typeof(Edge))]
	public class EdgeProperty : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			// Create property container element.
			var container = new VisualElement();

			// Create property fields.
			var startEndField = new PropertyField(property.FindPropertyRelative("StartEnd"));
			var splineField = new PropertyField(property.FindPropertyRelative("Spline"));

			// Add fields to the container.
			container.Add(startEndField);
			container.Add(splineField);

			return container;
		}
	}
}

