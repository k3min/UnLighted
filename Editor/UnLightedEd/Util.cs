using UnityEditor;
using UnityEngine;

namespace UnLightedEd
{
	internal static class Util
	{
		public static void Hint(bool cond, string hint, MessageType type = MessageType.Warning)
		{
			if (cond)
			{
				return;
			}

			EditorGUILayout.HelpBox(hint, type);
		}

		public static void Hint<T>(this Object t, System.Predicate<T> cond, string hint, MessageType type = MessageType.Warning) where T : Object
		{
			Util.Hint(cond(t as T), hint, type);
		}

		public static void Save(this Object t)
		{
			if (!GUI.changed)
			{
				return;
			}

			EditorUtility.SetDirty(t);
		}

		public static void DefaultInspector(this Editor editor)
		{
			editor.serializedObject.Update();

			var i = editor.serializedObject.GetIterator();

			i.NextVisible(true);

			while (i.NextVisible(true))
			{
				EditorGUILayout.PropertyField(i, true);
			}

			editor.serializedObject.ApplyModifiedProperties();
		}
	}
}