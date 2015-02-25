using UnityEngine;

namespace UnLighted
{
	[AddComponentMenu("UnLighted/Hand")]
	public class Hand : MonoBehaviour
	{
		private const int solverCont = 30;

		public float MaxDistance = 2;
		public float Break = 1000;

		private Joint joint;

		public Rigidbody ConnectedBody
		{
			get
			{
				return this.joint != null ? this.joint.connectedBody : null;
			}
		}

		private void Update()
		{
			if (!Input.GetButtonDown("Hand"))
			{
				return;
			}

			if (this.joint != null && this.joint.connectedBody != null)
			{
				this.joint.connectedBody.solverIterationCount = Physics.solverIterationCount;
				this.joint.connectedBody = null;

				return;
			}

			RaycastHit hit;

			var origin = Camera.main.transform.position;
			var direction = Camera.main.transform.forward;

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
				this.rigidbody.solverIterationCount = Hand.solverCont;
				this.rigidbody.hideFlags = HideFlags.HideInInspector;
			}

			this.joint.connectedBody = hit.rigidbody;
			this.joint.connectedBody.solverIterationCount = Hand.solverCont;
		}
	}
}