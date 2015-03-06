using UnityEngine;
using UnityEditor;
using UnLighted;

namespace UnLightedEd
{
	[CustomEditor(typeof(Platform))]
	internal class PlatformEditor : Editor
	{
		private SerializedProperty toP;
		private SerializedProperty speedP;
		private SerializedProperty smoothP;
		private SerializedProperty motionP;
		private SerializedProperty normalizeP;
		private SerializedProperty tagP;
		private SerializedProperty collisionP;

		private void OnEnable()
		{
			this.toP = this.serializedObject.FindProperty("To");
			this.speedP = this.serializedObject.FindProperty("Speed");
			this.smoothP = this.serializedObject.FindProperty("Smooth");
			this.motionP = this.serializedObject.FindProperty("Motion");
			this.normalizeP = this.serializedObject.FindProperty("Normalize");
			this.tagP = this.serializedObject.FindProperty("Tag");
			this.collisionP = this.serializedObject.FindProperty("Collision");
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.toP);
			EditorGUILayout.PropertyField(this.speedP);
			EditorGUILayout.PropertyField(this.motionP);

			this.normalizeP.boolValue = GUILayout.Toggle(this.normalizeP.boolValue, "Normalize", EditorStyles.miniButton);
			this.smoothP.boolValue = GUILayout.Toggle(this.smoothP.boolValue, "Smooth", EditorStyles.miniButton);

			EditorGUILayout.Separator();

			GUI.enabled = this.motionP.enumValueIndex != (int)PlatformMotion.Default;

			this.collisionP.boolValue = GUILayout.Toggle(this.collisionP.boolValue, "Collision", EditorStyles.miniButton);

			GUI.enabled &= this.collisionP.boolValue;

			this.tagP.stringValue = EditorGUILayout.TagField("Tag", this.tagP.stringValue);

			GUI.enabled = true;

			this.target.Hint<Platform>(x => x.rigidbody.isKinematic, "You may want to make the rigidbody kinematic", MessageType.Info);

			this.serializedObject.ApplyModifiedProperties();
		}

		private void OnSceneGUI()
		{
			var platform = this.target as Platform;
			var point = platform.transform.TransformPoint(platform.To);

			point = Handles.PositionHandle(point, Quaternion.identity);

			Handles.DrawLine(platform.transform.position, point);

			Undo.RecordObject(this.target, "Move Point");

			platform.To = platform.transform.InverseTransformPoint(point);

			platform.To.x = (float)System.Math.Round(platform.To.x, 4);
			platform.To.y = (float)System.Math.Round(platform.To.y, 4);
			platform.To.z = (float)System.Math.Round(platform.To.z, 4);

			platform.Save();
		}
	}
}