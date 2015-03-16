using UnityEngine;
using UnLighted;
using System.Linq;

namespace UnLighted.Managers
{
	[AddComponentMenu("UnLighted/Managers/Game Manager")]
	public class GameManager : ManagerBase<GameManager>
	{
		[HideInInspector, SerializeField]
		private int index;

		[HideInInspector]
		public GameState[] States;

		public GameState State
		{
			get
			{
				return this.States.ElementAtOrDefault(this.index);
			}

			set
			{
				this.State.End();

				this.index = System.Array.IndexOf<GameState>(this.States, value);

				this.State.Start();
			}
		}

		public virtual void Awake()
		{
			this.State.Start();
		}

		public virtual void Update()
		{
			this.State.Update();
		}
	}
}