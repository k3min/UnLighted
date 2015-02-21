using UnityEditor;
using UnityEngine;
using UnLighted.Triggers;

[CustomEditor(typeof(Trigger), true)]
public class TriggerEditor : TriggerBaseEditor
{
	private SerializedProperty toggleP;
	private SerializedProperty audioP;

	public override void OnEnable()
	{
		base.OnEnable();

		this.toggleP = this.serializedObject.FindProperty("Toggle");
		this.audioP = this.serializedObject.FindProperty("Audio");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();

		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(this.audioP);

		this.toggleP.boolValue = GUILayout.Toggle(this.toggleP.boolValue, "Toggle", EditorStyles.miniButton);

		EditorGUILayout.Space();

		this.serializedObject.ApplyModifiedProperties();

		base.OnInspectorGUI();
	}
}