using UnityEditor;
using UnityEngine;
using UnLighted;
using UnLighted.Managers;
using System.Linq;

namespace UnLightedEd.Managers
{
	[CustomEditor(typeof(GameManager), true)]
	internal class GameManagerEditor : Editor
	{
		private SerializedProperty iP;
		private SerializedProperty sP;

		private void OnEnable()
		{
			this.iP = this.serializedObject.FindProperty("index");
			this.sP = this.serializedObject.FindProperty("States");
		}

		public override void OnInspectorGUI()
		{
			this.DefaultInspector();

			var a = GameStateEditor.FindAll();
			var l = a.Length;

			this.serializedObject.Update();

			this.sP.arraySize = l;

			if (l > 0)
			{
				for (var j = 0; j < l; j++)
				{
					this.sP.GetArrayElementAtIndex(j).objectReferenceValue = a[j];
				}

				var d = a.Select(x => x.name).ToArray();
				var i = EditorGUILayout.Popup("State", this.iP.intValue, d);
				var s = this.sP.GetArrayElementAtIndex(i).objectReferenceValue;

				this.iP.intValue = i;

				Editor.CreateEditor(s).OnInspectorGUI();
			}

			this.serializedObject.ApplyModifiedProperties();

			Util.Hint(l > 0, "No GameStates!");
			Util.Hint(l > 0, "Right-click on a GameState script and select \"Create > Game State\"", MessageType.Info);
		}
	}
}