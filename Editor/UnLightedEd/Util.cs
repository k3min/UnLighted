using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace UnLightedEd
{
	internal static class Util
	{
		private static readonly Dictionary<TextAnchor, GUIStyle> label = new Dictionary<TextAnchor, GUIStyle>();
		private static GUIStyle richLabel;

		public static GUIStyle AlignedLabel(TextAnchor a)
		{
			GUIStyle s;

			Util.label.TryGetValue(a, out s);

			return s ?? (Util.label[a] = new GUIStyle(EditorStyles.label) { alignment = a });
		}

		public static GUIStyle RichLabel
		{
			get
			{
				if (Util.richLabel == null)
				{
					Util.richLabel = new GUIStyle(EditorStyles.label) { richText = true };
				}

				return Util.richLabel;
			}
		}

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

			while (i.NextVisible(false))
			{
				EditorGUILayout.PropertyField(i, true);
			}

			editor.serializedObject.ApplyModifiedProperties();
		}
	}
}