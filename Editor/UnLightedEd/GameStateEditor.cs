using UnityEditor;
using UnityEngine;
using UnLighted;
using System.Linq;

namespace UnLightedEd
{
	[CustomEditor(typeof(GameState), true)]
	internal class GameStateEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			this.DefaultInspector();
		}

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
			var active = Selection.activeObject as MonoScript;
			var type = active.GetClass();

			return active != null && type.IsSubclassOf(typeof(GameState));
		}

		public static GameState[] FindAll()
		{
			return AssetDatabase.FindAssets("t:GameState").Select(x => GameStateEditor.FromGUID(x)).ToArray();
		}

		private static GameState FromGUID(string guid)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			var asset = AssetDatabase.LoadAssetAtPath(path, typeof(GameState));

			return asset as GameState;
		}
	}
}

