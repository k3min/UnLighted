using UnityEditor;
using UnityEngine;
using System.Linq;
using UnLighted.Triggers;

[CustomEditor(typeof(TriggerBase), true)]
public class TriggerBaseEditor : Editor
{
	private SerializedProperty targetsP;

	public virtual void OnEnable()
	{
		this.targetsP = this.serializedObject.FindProperty("Targets");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();

		EditorGUILayout.LabelField("Targets");

		for (var i = 0; i < this.targetsP.arraySize; i++)
		{
			var p = this.targetsP.GetArrayElementAtIndex(i);

			var stateP = p.FindPropertyRelative("StartState");
			var goP = p.FindPropertyRelative("GameObject");
			var actionP = p.FindPropertyRelative("Action");

			EditorGUILayout.PropertyField(goP, GUIContent.none);

			var go = goP.objectReferenceValue as GameObject;

			if (go != null)
			{
				var behaviourP = p.FindPropertyRelative("Behaviour");
				var behaviours = go.GetComponents<Behaviour>();

				if (behaviours.Length > 0)
				{
					var behaviour = behaviourP.objectReferenceValue as Behaviour;
					var index = Mathf.Max(behaviours.ToList().IndexOf(behaviour), 0);
					var strings = behaviours.Select(x => x.GetType().ToString()).ToArray();

					behaviourP.objectReferenceValue = behaviours[EditorGUILayout.Popup(index, strings)];

					EditorGUILayout.PropertyField(actionP, GUIContent.none);
				}
				else
				{
					EditorGUILayout.HelpBox("GameObject has no Behaviours!", MessageType.Warning);
				}
			}

			EditorGUILayout.BeginHorizontal();
			{
				stateP.boolValue = GUILayout.Toggle(stateP.boolValue, stateP.boolValue ? "On" : "Off", EditorStyles.miniButtonLeft);

				if (GUILayout.Button("Delete", EditorStyles.miniButtonRight))
				{
					this.targetsP.DeleteArrayElementAtIndex(i);
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
		}

		if (GUILayout.Button("Add"))
		{
			this.targetsP.InsertArrayElementAtIndex(this.targetsP.arraySize);
		}

		var trigger = this.target as TriggerBase;

		if (trigger.collider != null && !trigger.collider.isTrigger)
		{
			EditorGUILayout.HelpBox("Collider isn't a trigger!", MessageType.Warning);
		}

		this.serializedObject.ApplyModifiedProperties();
	}

	public virtual void OnSceneGUI()
	{
		var trigger = this.target as TriggerBase;

		if (trigger.Targets == null)
		{
			return;
		}

		foreach (var triggerTarget in trigger.Targets)
		{
			if (triggerTarget.GameObject != null)
			{
				Handles.DrawLine(trigger.transform.position, triggerTarget.GameObject.transform.position);
			}
		}
	}
}