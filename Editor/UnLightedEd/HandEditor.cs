using UnityEditor;
using UnityEngine;
using UnLighted;

namespace UnLightedEd
{
	[CustomEditor(typeof(Hand))]
	internal class HandEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			this.DefaultInspector();

			var trigger = this.target as Hand;

			if (trigger.Holding == null)
			{
				return;
			}

			GUI.enabled = false;

			EditorGUILayout.ObjectField("Holding", trigger.Holding, typeof(Rigidbody), false);

			GUI.enabled = true;
		}
	}
}