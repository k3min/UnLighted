using UnityEngine;

namespace UnLighted
{
	public abstract class GameState : ScriptableObject
	{
		public virtual void Start()
		{
		}

		public virtual void Update()
		{
		}

		public virtual void End()
		{
		}
	}
}