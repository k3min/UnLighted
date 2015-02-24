using UnityEngine;

namespace UnLighted
{
	[AddComponentMenu("UnLighted/Hand"), RequireComponent(typeof(Joint))]
	public class Hand : MonoBehaviour
	{
		public float Distance = 2;

		private Joint joint;

		private void Awake()
		{
			this.joint = this.GetComponent<Joint>();
		}

		private void Update()
		{
			RaycastHit hit;

			var origin = Camera.main.transform.position;
			var direction = Camera.main.transform.forward;

			if (!Physics.Raycast(origin, direction, out hit, this.Distance))
			{
				return;
			}

			if (!Input.GetButtonDown("Hand"))
			{
				return;
			}

			if (this.joint.connectedBody != null)
			{
				this.joint.connectedBody = null;
				return;
			}

			if (hit.rigidbody == null || hit.rigidbody.isKinematic)
			{
				return;
			}

			this.joint.connectedBody = hit.rigidbody;
		}
	}
}