using UnityEngine;

namespace UnLighted
{
	public static class Util
	{
		public static T Find<T>(HideFlags f = HideFlags.DontSave) where T : Component
		{
			return Object.FindObjectOfType<T>() ?? new GameObject { hideFlags = f }.AddComponent<T>();
		}

		public static T Get<T>(this Component t) where T : Component
		{
			return t.GetComponent<T>() ?? t.gameObject.AddComponent<T>();
		}
	}
}