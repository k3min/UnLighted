using UnityEditor;
using UnLighted;
using UnLighted.Managers;
using System.Linq;

namespace UnLightedEd.Managers
{
	[CustomEditor(typeof(GameManager))]
	internal class GameManagerEditor : Editor
	{
		private SerializedProperty indexP;
		private SerializedProperty statesP;

		private void OnEnable()
		{
			this.indexP = this.serializedObject.FindProperty("index");
			this.statesP = this.serializedObject.FindProperty("States");
		}

		public override void OnInspectorGUI()
		{
			var states = GameStateEditor.FindAll();
			var length = states.Length;

			this.serializedObject.Update();

			this.statesP.arraySize = length;

			for (var i = 0; i < length; i++)
			{
				this.statesP.GetArrayElementAtIndex(i).objectReferenceValue = states[i];
			}

			if (length > 0)
			{
				var i = this.indexP.intValue;
				var d = states.Select(x => x.name).ToArray();

				i = EditorGUILayout.Popup("State", i, d);

				this.indexP.intValue = i;

				var s = this.statesP.GetArrayElementAtIndex(i).objectReferenceValue;

				Editor.CreateEditor(s).OnInspectorGUI();
			}

			this.serializedObject.ApplyModifiedProperties();

			Util.Hint(states.Length > 0, "No GameStates!");
			Util.Hint(states.Length > 0, "Right-click on a GameState script and select \"Create > Game State\"", MessageType.Info);
		}
	}
}