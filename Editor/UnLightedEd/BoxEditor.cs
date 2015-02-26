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
			base.OnInspectorGUI();

			this.target.Hint<Box>(x => LayerMask.LayerToName(x.gameObject.layer) == "Box", "GameObject isn't in the \"Box\" layer!");
			this.target.Hint<Box>(x => x.collider.isTrigger, "Collider isn't a trigger!");

			GUI.enabled = EditorApplication.isPlaying;

			if (GUILayout.Button("Render"))
			{
				BoxProbe.Render(this.target as Box);
			}

			GUI.enabled = true;
		}
	}
}