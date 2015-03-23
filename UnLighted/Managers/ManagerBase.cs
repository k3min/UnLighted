using UnityEngine;

namespace UnLighted.Managers
{
	public abstract class ManagerBase<T> : MonoBehaviour where T : ManagerBase<T>
	{
		private static T main;

		public static T Main
		{
			get
			{
				if (ManagerBase<T>.main == null)
				{
					ManagerBase<T>.main = Util.Find<T>();
				}

				return ManagerBase<T>.main;
			}
		}
	}
}