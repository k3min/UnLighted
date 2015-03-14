using UnityEngine;

namespace UnLighted
{
	[System.Serializable]
	public abstract class GameState : ScriptableObject
	{
		public abstract void Start();

		public abstract void Update();

		public abstract void End();
	}
}