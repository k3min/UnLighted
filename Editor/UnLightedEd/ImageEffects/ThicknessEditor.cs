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

			EditorGUILayout.HelpBox("Only renderers in the \"TransparentFX\" layer will be rendered", MessageType.Info);

			this.target.Hint<Thickness>(x => x.camera.tag != "MainCamera", "This shouldn't be on the main camera!");
		}

		public static void Hide(Object target)
		{
			var camera = (target as Thickness).camera;

			camera.depth = -2;
			camera.hideFlags = HideFlags.HideInHierarchy;
		}
	}
}