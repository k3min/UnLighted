using UnityEditor;
using UnityEngine;

namespace UnLightedEd
{
	public static class Util
	{
		public static void Hint<T>(Object target, System.Predicate<T> cond, string hint, MessageType message) where T : Behaviour
		{
			if (cond(target as T))
			{
				EditorGUILayout.HelpBox(hint, message);
			}
		}
	}
}

