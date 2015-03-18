using UnityEditor;
using UnLighted.Controllers;
using UnityEngine;

namespace UnLightedEd.Controllers
{
	[CustomEditor(typeof(CameraController))]
	internal class CameraControllerEditor : Editor
	{
		private SerializedProperty minP;
		private SerializedProperty maxP;

		private void OnEnable()
		{
			this.minP = this.serializedObject.FindProperty("Min");
			this.maxP = this.serializedObject.FindProperty("Max");
		}

		public override void OnInspectorGUI()
		{
			this.DefaultInspector();

			this.serializedObject.Update();

			var label = EditorGUIUtility.labelWidth;

			var rect = EditorGUILayout.GetControlRect(true, 32);
			var width = rect.width;

			rect.width = label - 1;
			rect.height = 16;

			GUI.Label(rect, "Range");

			rect.width = width;
			rect.xMin += label - 1;

			EditorGUIUtility.labelWidth = 30;

			EditorGUI.PropertyField(rect, this.minP);

			rect.y += 16;

			EditorGUI.PropertyField(rect, this.maxP);

			EditorGUIUtility.labelWidth = label;

			this.serializedObject.ApplyModifiedProperties();
		}
	}
}