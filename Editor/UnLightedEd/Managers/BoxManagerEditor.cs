using UnityEditor;
using UnityEngine;
using UnLighted;
using UnLighted.Managers;
using System.Collections.Generic;

namespace UnLightedEd.Managers
{
	[CustomEditor(typeof(BoxManager))]
	internal class BoxManagerEditor : Editor
	{
		private Box[] boxes;

		private static Dictionary<Box, GUIContent> content = new Dictionary<Box, GUIContent>();

		private void OnEnable()
		{
			this.boxes = Object.FindObjectsOfType<Box>();
		}

		public override void OnInspectorGUI()
		{
			foreach (var box in this.boxes)
			{
				GUILayout.Label(BoxManagerEditor.Content(box), Util.AlignedLabel(TextAnchor.MiddleLeft));
			}

			Util.Hint(LayerMask.NameToLayer("Box") != -1, "The \"Box\" layer doesn't exist!");
		}

		private void OnSceneGUI()
		{
			if (this.boxes != null)
			{
				foreach (var box in this.boxes)
				{
					Handles.Label(box.Pos, BoxManagerEditor.Content(box), Util.AlignedLabel(TextAnchor.MiddleCenter));
				}
			}
		}

		private static GUIContent Content(Box b)
		{
			GUIContent c;

			BoxManagerEditor.content.TryGetValue(b, out c);

			return c ?? (BoxManagerEditor.content[b] = new GUIContent(b.name, AssetPreview.GetMiniThumbnail(b.Cubemap)));
		}
	}
}