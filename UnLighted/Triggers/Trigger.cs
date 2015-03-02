using UnityEngine;
using System.Collections.Generic;

namespace UnLighted.Triggers
{
	[AddComponentMenu("UnLighted/Triggers/Trigger"), RequireComponent(typeof(Collider))]
	public class Trigger : TriggerBase
	{
		private List<Collider> others = new List<Collider>();

		[HideInInspector]
		public bool Toggle;

		public override void Awake()
		{
			base.Awake();

			var bounds = new Bounds();

			for (var i = 0; i < this.Targets.Length; i++)
			{
				var go = this.Targets[i].GameObject;

				if (go == null)
				{
					continue;
				}

				var ren = go.renderer;

				if (ren == null)
				{
					continue;
				}

				if (i == 0)
				{
					bounds = ren.bounds;
				}
				else
				{
					bounds.Encapsulate(ren.bounds);
				}
			}
		}

		public virtual void OnTriggerEnter(Collider other)
		{
			if (this.others.Contains(other))
			{
				return;
			}

			this.others.Add(other);

			if (this.others.Count != 1)
			{
				return;
			}

			foreach (var t in this.Targets)
			{
				t.State = this.Toggle ? !t.State : !t.StartState;
			}
		}

		public virtual void OnTriggerExit(Collider other)
		{
			if (this.others.Contains(other))
			{
				this.others.Remove(other);
			}

			if (this.others.Count > 0 || this.Toggle)
			{
				return;
			}

			foreach (var t in this.Targets)
			{
				t.State = t.StartState;
			}
		}
	}
}