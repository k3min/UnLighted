using UnityEditor;
using UnityEngine;
using UnLighted;
using UnLighted.ImageEffects;

namespace UnLightedEd
{
	[CustomEditor(typeof(Box))]
	internal class BoxEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var box = base.target as Box;

			base.OnInspectorGUI();

			if (LayerMask.LayerToName(box.gameObject.layer) != "Box")
			{
				EditorGUILayout.HelpBox("GameObject isn't in the \"Box\" layer!", MessageType.Warning);
			}

			Util.Hint<Box>(this.target, x => !x.collider.isTrigger, "Collider isn't a trigger!", MessageType.Warning);

			GUI.enabled = EditorApplication.isPlaying;

			if (GUILayout.Button("Render"))
			{
				BoxProbe.Render(box);
			}

			GUI.enabled = true;
		}
	}
}