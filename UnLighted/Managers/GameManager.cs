using UnityEngine;
using UnLighted;
using System.Linq;

namespace UnLighted.Managers
{
	[AddComponentMenu("UnLighted/Managers/Game Manager")]
	public class GameManager : ManagerBase<GameManager>
	{
		[SerializeField]
		private int index;

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

		private void Awake()
		{
			if (this.State == null)
			{
				return;
			}

			this.State.Start();
		}

		private void Update()
		{
			if (this.State == null)
			{
				return;
			}

			this.State.Update();
		}
	}
}