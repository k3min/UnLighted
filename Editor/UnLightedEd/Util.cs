using UnityEditor;
using UnityEngine;

namespace UnLightedEd
{
	internal static class Util
	{
		public static void Hint<T>(this Object t, System.Predicate<T> cond, string hint, MessageType type = MessageType.Warning) where T : Object
		{
			if (!cond(t as T))
			{
				EditorGUILayout.HelpBox(hint, type);
			}
		}

		public static void Save(this Object t)
		{
			if (GUI.changed)
			{
				EditorUtility.SetDirty(t);
			}
		}
	}
}