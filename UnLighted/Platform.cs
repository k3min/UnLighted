using UnityEngine;

namespace UnLighted
{
	public enum PlatformMotion
	{
		Default,
		A2B,
		AutoBack
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
		public PlatformMotion Motion;
		public float Speed = 1;
		public bool Normalize = true;
		public string Tag = "Untagged";

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
				motion /= Vector3.Distance(this.a, this.b);
			}

			if (this.Motion != PlatformMotion.Default)
			{
				motion *= this.dir;
			}

			this.time += motion * Time.fixedDeltaTime;

			if (this.Motion == PlatformMotion.Default)
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
			if (this.Motion == PlatformMotion.Default || this.Tag != other.gameObject.tag)
			{
				return;
			}

			if (this.time <= float.Epsilon)
			{
				this.OnTrigger(true);
			}

			if (this.time >= (1f - float.Epsilon))
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
			if (this.Motion != PlatformMotion.AutoBack ||
			    this.Tag != other.gameObject.tag ||
			    this.time <= float.Epsilon ||
			    this.time >= (1f - float.Epsilon))
			{
				return;
			}

			this.dir *= -1;
		}
	}
}