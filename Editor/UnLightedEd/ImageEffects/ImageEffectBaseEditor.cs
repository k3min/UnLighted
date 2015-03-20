using UnityEditor;
using UnityEngine;
using UnLighted.ImageEffects;
using System.Linq;
using System.Reflection;

namespace UnLightedEd.ImageEffects
{
	[CustomEditor(typeof(ImageEffectBase), true)]
	internal class ImageEffectBaseEditor : Editor
	{
		private SerializedProperty debugP;

		private void OnEnable()
		{
			this.debugP = this.serializedObject.FindProperty("Debug");
		}

		public override void OnInspectorGUI()
		{
			this.DefaultInspector();

			this.serializedObject.Update();

			if (this.debugP != null)
			{
				this.debugP.boolValue = GUILayout.Toggle(this.debugP.boolValue, "Debug", EditorStyles.miniButton);
			}

			this.serializedObject.ApplyModifiedProperties();

			var blit = this.target.GetType().GetMethod("OnRenderImage");
			var attr = blit.GetCustomAttributes(true).FirstOrDefault() as System.Attribute;

			if (attr != null)
			{
				EditorGUILayout.HelpBox(attr.GetType().Name, MessageType.Info);
			}
		}
	}
}