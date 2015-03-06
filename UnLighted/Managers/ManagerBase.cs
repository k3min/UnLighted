﻿using UnityEngine;

namespace UnLighted.Managers
{
	[AddComponentMenu("")]
	public abstract class ManagerBase<T> : MonoBehaviour where T : ManagerBase<T>
	{
		private static T main;

		public static T Main
		{
			get
			{
			    if (ManagerBase<T>.main != null)
			    {
			        return ManagerBase<T>.main;
			    }

			    ManagerBase<T>.main = Object.FindObjectOfType<T>();

			    if (ManagerBase<T>.main != null)
			    {
			        return ManagerBase<T>.main;
			    }

			    var gameObject = new GameObject
			    {
			        hideFlags = HideFlags.DontSave
			    };

			    ManagerBase<T>.main = gameObject.AddComponent<T>();

			    return ManagerBase<T>.main;
			}
		}
	}
}
