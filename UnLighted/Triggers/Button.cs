using UnityEngine;

namespace UnLighted.Triggers
{
	[AddComponentMenu("UnLighted/Triggers/Button")]
	public class Button : TriggerBase
	{
		private void OnMouseDown()
		{
			if (!this.enabled)
			{
				return;
			}

			foreach (var t in this.Targets)
			{
				t.State = !t.State;
			}
		}

		private void OnDisable()
		{
			foreach (var t in this.Targets)
			{
				if (t.GameObject == this.gameObject)
				{
					continue;
				}

				t.State = t.StartState;
			}
		}
	}
}