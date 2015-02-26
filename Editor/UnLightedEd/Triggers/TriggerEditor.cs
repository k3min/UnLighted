using UnityEditor;
using UnityEngine;
using UnLighted.Triggers;

namespace UnLightedEd.Triggers
{
	[CustomEditor(typeof(Trigger), true)]
	internal class TriggerEditor : TriggerBaseEditor
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
}