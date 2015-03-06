using UnityEditor;
using UnityEngine;
using UnLighted.Managers;
using UnLighted;

namespace UnLightedEd.Managers
{
	[CustomEditor(typeof(BoxManager))]
	internal class BoxManagerEditor : Editor
	{
		private Box[] boxes;

		private void Awake()
		{
			this.boxes = Object.FindObjectsOfType<Box>();
		}

		public override void OnInspectorGUI()
		{
			if (this.boxes != null)
			{
				GUI.enabled = false;

				foreach (var box in this.boxes)
				{
					EditorGUILayout.ObjectField(box.name, box, typeof(Box), true);
				}

				GUI.enabled = true;
			}

			Util.Hint(LayerMask.NameToLayer("Box") != -1, "The \"Box\" layer doesn't exist!");
		}
	}
}