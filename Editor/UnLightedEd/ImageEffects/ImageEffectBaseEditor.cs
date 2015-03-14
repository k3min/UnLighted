using UnityEditor;
using UnityEngine;
using UnLighted.ImageEffects;

namespace UnLightedEd.ImageEffects
{
	[CustomEditor(typeof(ImageEffectBase), true)]
	internal class ImageEffectBaseEditor : Editor
	{
		private SerializedProperty debugP;

		private void Awake()
		{
			this.debugP = this.serializedObject.FindProperty("Debug");
		}

		public override void OnInspectorGUI()
		{
			this.DefaultInspector();

			if (this.debugP != null)
			{
				this.serializedObject.Update();

				this.debugP.boolValue = GUILayout.Toggle(this.debugP.boolValue, "Debug", EditorStyles.miniButton);

				this.serializedObject.ApplyModifiedProperties();
			}

			var depth = (this.target as ImageEffectBase).Depth;

			Util.Hint(depth == DepthTextureMode.None, "DepthTextureMode: " + depth, MessageType.Info);
		}
	}
}