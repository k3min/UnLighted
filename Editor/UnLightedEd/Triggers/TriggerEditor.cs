using UnityEditor;
using UnityEngine;
using UnLighted.Triggers;

[CustomEditor(typeof(Trigger), true)]
public class TriggerEditor : TriggerBaseEditor
{
	private SerializedProperty toggleP;

	public override void OnEnable()
	{
		base.OnEnable();

		this.toggleP = this.serializedObject.FindProperty("Toggle");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();

		this.toggleP.boolValue = GUILayout.Toggle(this.toggleP.boolValue, "Toggle", EditorStyles.miniButton);

		this.serializedObject.ApplyModifiedProperties();

		base.OnInspectorGUI();
	}
}