using UnityEngine;

namespace UnLighted.Triggers
{
	public enum TriggerAction
	{
		EnableDisable,
		SendOnTrigger
	}

	[AddComponentMenu("")]
	public abstract class TriggerBase : MonoBehaviour
	{
		[HideInInspector]
		public TriggerTarget[] Targets;

		public virtual void Awake()
		{
			foreach (var t in this.Targets)
			{
				t.State = t.StartState;
			}
		}

		public virtual void Update()
		{
		}
	}

	[System.Serializable]
	public class TriggerTarget
	{
		[SerializeField]
		private bool state;

		public GameObject GameObject;
		public Behaviour Behaviour;
		public TriggerAction Action;
		public bool StartState;

		public bool State
		{
			get { return this.state; }
			set
			{
				this.state = value;

				if (this.Behaviour == null)
				{
					return;
				}

				switch (this.Action)
				{
					case TriggerAction.EnableDisable:
						this.Behaviour.enabled = this.state;
						break;

					case TriggerAction.SendOnTrigger:
						this.Behaviour.SendMessage("OnTrigger", this.state);
						break;
				}
			}
		}
	}
}