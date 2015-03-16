using UnityEditor;
using UnityEngine;
using UnLighted;
using System.Linq;

namespace UnLightedEd
{
	[CustomEditor(typeof(GameState), true)]
	internal class GameStateEditor : Editor
	{
		[MenuItem("Assets/Create/Game State")]
		private static void Create()
		{
			var name = Selection.activeObject.name;
			var asset = ScriptableObject.CreateInstance(name);

			ProjectWindowUtil.CreateAsset(asset, name + ".asset");
		}

		[MenuItem("Assets/Create/Game State", true)]
		private static bool Validate()
		{
			var type = typeof(GameState);
			var active = Selection.activeObject as MonoScript;

			return active != null && active.GetClass().IsSubclassOf(type);
		}

		public override void OnInspectorGUI()
		{
			this.DefaultInspector();
		}

		public static GameState[] FindAll()
		{
			return AssetDatabase.FindAssets("t:" + typeof(GameState).Name).Select(GameStateEditor.FromGUID).ToArray();
		}

		private static GameState FromGUID(string guid)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			var asset = AssetDatabase.LoadAssetAtPath(path, typeof(GameState));

			return asset as GameState;
		}
	}
}

