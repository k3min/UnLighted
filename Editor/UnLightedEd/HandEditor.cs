using UnityEditor;
using UnityEngine;
using UnLighted;

namespace UnLightedEd
{
	[CustomEditor(typeof(Hand))]
	public class HandEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var trigger = this.target as Hand;

			if (trigger.ConnectedBody != null)
			{
				EditorGUILayout.Separator();
				EditorGUILayout.ObjectField("Holding", trigger.ConnectedBody, typeof(Rigidbody), false);
			}
		}
	}
}