using UnityEditor;
using UnityEngine;
using UnLighted.ImageEffects;

namespace UnLightedEd.ImageEffects
{
	[CustomEditor(typeof(Thickness))]
	internal class ThicknessEditor : Editor
	{
		private void OnEnable()
		{
			ThicknessEditor.Hide(this.target);
		}

		public override void OnInspectorGUI()
		{
			this.DefaultInspector();

			this.target.Hint<Thickness>(x => x.camera.tag != "MainCamera", "This shouldn't be on the main camera!");
		}

		public static void Hide(Object target)
		{
			var camera = (target as Thickness).camera;

			camera.enabled = false;
			camera.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy;
		}
	}
}