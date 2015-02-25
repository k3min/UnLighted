using UnityEngine;

namespace UnLighted
{
	public enum Motion
	{
		Default,
		A2B,
		Back
	}

	[AddComponentMenu("UnLighted/Platform"), RequireComponent(typeof(Rigidbody))]
	public class Platform : MonoBehaviour
	{
		private float time;
		private int dir;
		private Vector3 a;
		private Vector3 b;
		public Vector3 To;
		public bool Smooth;
		public Motion Motion;
		public float Speed = 1;
		public bool Collision;
		public bool Normalize = true;

		public float T { get; private set; }

		private void Awake()
		{
			this.a = this.transform.position;
			this.b = this.transform.TransformPoint(this.To);
		}

		private void FixedUpdate()
		{
			var motion = this.Speed;

			if (this.Normalize)
			{
				motion *= 1f / Vector3.Distance(this.a, this.b);
			}

			if (this.Motion != Motion.Default)
			{
				motion *= this.dir;
			}

			this.time += motion * Time.fixedDeltaTime;

			if (this.Motion == Motion.Default)
			{
				this.time %= 1;
			}
			else
			{
				this.time = Mathf.Clamp01(this.time);
			}

			this.T = this.Smooth ? (1f - Mathf.Cos(this.time * Mathf.PI)) * 0.5f : this.time;

			this.rigidbody.MovePosition(Vector3.Lerp(this.a, this.b, this.T));
		}

		private void OnCollisionEnter(Collision other)
		{
			if (!this.Collision || this.Motion != Motion.Default || other.gameObject.tag != "Player")
			{
				return;
			}

			if (Mathf.Abs(this.time) < Mathf.Epsilon)
			{
				this.OnTrigger(true);
			}

			if (Mathf.Abs(this.time - 1) < Mathf.Epsilon)
			{
				this.OnTrigger(false);
			}
		}

		public void OnTrigger(bool state)
		{
			this.dir = state ? 1 : -1;
		}

		private void OnCollisionExit(Collision other)
		{
			if (this.Collision &&
			    this.Motion == Motion.Back &&
			    other.gameObject.tag == "Player" &&
			    Mathf.Abs(this.time) > Mathf.Epsilon &&
			    Mathf.Abs(this.time - 1) > Mathf.Epsilon)
			{
				this.dir *= -1;
			}
		}
	}
}