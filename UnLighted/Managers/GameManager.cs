using UnityEngine;

namespace UnLighted.Managers
{
	[AddComponentMenu("UnLighted/Managers/Game Manager")]
	public class GameManager : ManagerBase<GameManager>
	{
		public virtual void Update()
		{
			Screen.lockCursor = true;
		}
	}
}