using UnityEngine;
using UnLighted.Controllers;

namespace UnLighted
{
	[AddComponentMenu("UnLighted/Hand")]
	public class Hand : MonoBehaviour
	{
		private Joint joint;

		public float MaxDistance = 2;
		public float Break = 1000;

		public Rigidbody Holding
		{
			get
			{
				return this.joint != null ? this.joint.connectedBody : null;
			}

			set
			{
				if (this.joint == null)
				{
					return;
				}

				this.joint.connectedBody = value;
			}
		}

		private void Update()
		{
			if (!Input.GetButtonDown("Hand"))
			{
				return;
			}

			if (this.Holding != null)
			{
				this.Holding.solverIterationCount = Physics.solverIterationCount;
				this.Holding = null;

				return;
			}

			var origin = Camera.main.transform.position;
			var direction = Camera.main.transform.forward;

			RaycastHit hit;

			if (!Physics.Raycast(origin, direction, out hit, this.MaxDistance))
			{
				return;
			}

			if (hit.rigidbody == null || hit.rigidbody.isKinematic)
			{
				return;
			}

			if (this.joint == null)
			{
				this.joint = this.gameObject.AddComponent<FixedJoint>();
				this.joint.breakTorque = this.Break;
				this.joint.hideFlags = HideFlags.HideInInspector;

				this.rigidbody.isKinematic = true;
				this.rigidbody.solverIterationCount = PlayerController.SolverCount;
				this.rigidbody.hideFlags = HideFlags.HideInInspector;
			}

			this.Holding = hit.rigidbody;
			this.Holding.solverIterationCount = PlayerController.SolverCount;
		}
	}
}