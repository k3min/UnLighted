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
		private SerializedProperty collisionP;
		private SerializedProperty normalizeP;

		private void OnEnable()
		{
			this.toP = this.serializedObject.FindProperty("To");
			this.speedP = this.serializedObject.FindProperty("Speed");
			this.smoothP = this.serializedObject.FindProperty("Smooth");
			this.motionP = this.serializedObject.FindProperty("Motion");
			this.collisionP = this.serializedObject.FindProperty("Collision");
			this.normalizeP = this.serializedObject.FindProperty("Normalize");
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.toP);
			EditorGUILayout.PropertyField(this.speedP);
			EditorGUILayout.PropertyField(this.motionP);

			this.smoothP.boolValue = GUILayout.Toggle(this.smoothP.boolValue, "Smooth", EditorStyles.miniButton);
			this.collisionP.boolValue = GUILayout.Toggle(this.collisionP.boolValue, "Collision", EditorStyles.miniButton);
			this.normalizeP.boolValue = GUILayout.Toggle(this.normalizeP.boolValue, "Normalize", EditorStyles.miniButton);

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

			platform.To.x = Mathf.Round(platform.To.x * 100) * 0.01f;
			platform.To.y = Mathf.Round(platform.To.y * 100) * 0.01f;
			platform.To.z = Mathf.Round(platform.To.z * 100) * 0.01f;

			platform.Save();
		}
	}
}