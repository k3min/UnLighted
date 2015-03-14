using UnityEditor;
using UnityEngine;
using UnLighted.Managers;

namespace UnLightedEd.Managers
{
	[CustomEditor(typeof(ShaderManager))]
	internal class ShaderManagerEditor : Editor
	{
		private SerializedProperty propertiesP;

		private static GUIStyle label;

		private void OnEnable()
		{
			this.propertiesP = this.serializedObject.FindProperty("Properties");
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			for (var i = 0; i < this.propertiesP.arraySize; i++)
			{
				var p = this.propertiesP.GetArrayElementAtIndex(i);
				var typeP = p.FindPropertyRelative("Type");
				var typeI = (ShaderPropertyType)typeP.intValue;
				var type = System.Enum.GetName(typeof(ShaderPropertyType), typeI);

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PropertyField(typeP, GUIContent.none);

					if (GUILayout.Button("Delete", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
					{
						this.propertiesP.DeleteArrayElementAtIndex(i);
					}
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.PropertyField(p.FindPropertyRelative("Key"));

				switch (typeI)
				{
					case ShaderPropertyType.Color:
					case ShaderPropertyType.Float:
					case ShaderPropertyType.Integer:
					case ShaderPropertyType.Texture:
						EditorGUILayout.PropertyField(p.FindPropertyRelative(type));
						break;

					case ShaderPropertyType.Vector:
						var vecP = p.FindPropertyRelative("Vector");
						var vec = vecP.vector4Value;

						vec.x = ShaderManagerEditor.VectorField(p.FindPropertyRelative("X"), vec.x);
						vec.y = ShaderManagerEditor.VectorField(p.FindPropertyRelative("Y"), vec.y);
						vec.z = ShaderManagerEditor.VectorField(p.FindPropertyRelative("Z"), vec.z);
						vec.w = ShaderManagerEditor.VectorField(p.FindPropertyRelative("W"), vec.w);

						vecP.vector4Value = vec;

						break;

					case ShaderPropertyType.Keyword:
						EditorGUILayout.PropertyField(p.FindPropertyRelative("Enabled"));
						break;
				}

				EditorGUILayout.Space();
			}

			if (GUILayout.Button("Add"))
			{
				this.propertiesP.InsertArrayElementAtIndex(this.propertiesP.arraySize);
			}

			if (GUILayout.Button("Screenshot"))
			{
				Application.CaptureScreenshot(Time.frameCount + ".png");
			}

			this.serializedObject.ApplyModifiedProperties();
		}

		private static float VectorField(SerializedProperty property, float value)
		{
			if (ShaderManagerEditor.label == null)
			{
				ShaderManagerEditor.label = new GUIStyle(EditorStyles.label) { richText = true };
			}

			var width = EditorGUIUtility.labelWidth;

			EditorGUILayout.BeginHorizontal();
			{
				property.stringValue = GUILayout.TextField(property.stringValue, ShaderManagerEditor.label, GUILayout.Width(width));

				EditorGUIUtility.labelWidth = 13;

				value = EditorGUILayout.FloatField(property.name, value);

				EditorGUIUtility.labelWidth = width;
			}
			EditorGUILayout.EndHorizontal();

			return value;
		}
	}
}