using UnityEngine;

namespace UnLighted.Controllers
{
	[AddComponentMenu("UnLighted/Controllers/Player"), RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
	public class PlayerController : MonoBehaviour
	{
		public PlayerMovement Movement = new PlayerMovement
		{
			Speed = 3,
			Jump = 5,
			FeetRadius = 0.2f,
			Friction = 1,
			FrictionCombine = PhysicMaterialCombine.Maximum
		};

		public PlayerHeadBob HeadBob = new PlayerHeadBob
		{
			Offset = new Vector3(0.02f, 0.02f, 0.05f),
			Tilt = -10,
			FallScale = 0.1f,
			FallSmoothing = 0.3f,
			FallMax = 0.8f
		};

		private bool canWalk;
		private bool canJump;
		private bool jump;
		private bool grounded;

		private Vector3 previousPosition;
		private Vector3 velocity;
		private Vector3 direction;
		private Vector3 origin;

		private void Awake()
		{
			this.origin = Camera.main.transform.localPosition;
			base.rigidbody.freezeRotation = true;
			base.collider.sharedMaterial = new PhysicMaterial("Player")
			{
				dynamicFriction = this.Movement.Friction,
				staticFriction = this.Movement.Friction,
				frictionCombine = this.Movement.FrictionCombine
			};
		}

		private void FixedUpdate()
		{
			this.grounded = Physics.CheckSphere(base.rigidbody.position, this.Movement.FeetRadius, 1);

			if (this.canWalk)
			{
				base.rigidbody.MovePosition(base.rigidbody.position + (this.direction * this.Movement.Speed * Time.fixedDeltaTime));
			}

			if (this.jump)
			{
				this.jump = false;

				if (this.canJump)
				{
					base.rigidbody.AddForce(Vector3.up * this.Movement.Jump, ForceMode.VelocityChange);
				}
			}

			this.velocity = (base.rigidbody.position - this.previousPosition) / Time.fixedDeltaTime;
			this.previousPosition = base.rigidbody.position;
		}

		private void LateUpdate()
		{
			if (Camera.main == null)
			{
				return;
			}

			var bob = this.HeadBob.Bob(this.velocity);
			var rot = Camera.main.transform.localEulerAngles;

			rot.x += bob.y * this.HeadBob.Tilt;
			rot.z += bob.x * this.HeadBob.Tilt;

			var fwd = Quaternion.AngleAxis(rot.y, Vector3.up);

			Camera.main.transform.localPosition = this.origin + (fwd * bob);
			Camera.main.transform.localEulerAngles = rot;
		}

		private void OnCollisionExit(Collision other)
		{
			this.canJump = false;
		}

		private void OnCollisionStay(Collision other)
		{
			if (this.velocity.y < 0)
			{
				return;
			}

			this.canJump = this.grounded;

			if (other.rigidbody == null)
			{
				this.canWalk = this.grounded;
			}
			else
			{
				this.canWalk |= this.grounded;
			}
		}

		private void Update()
		{
			if (Camera.main == null)
			{
				return;
			}

			this.direction.x = Input.GetAxis("Horizontal");
			this.direction.z = Input.GetAxis("Vertical");

			var mag = this.direction.magnitude;

			if (mag > 1)
			{
				this.direction /= mag;
			}

			var fwd = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);

			this.direction = fwd * this.direction;

			this.jump |= Input.GetButtonDown("Jump");
		}
	}

	[System.Serializable]
	public struct PlayerMovement
	{
		public float Speed;
		public float Jump;
		public float FeetRadius;
		public float Friction;
		public PhysicMaterialCombine FrictionCombine;
	}

	[System.Serializable]
	public struct PlayerHeadBob
	{
		public Vector3 Offset;
		public float Tilt;
		public float FallScale;
		public float FallSmoothing;
		public float FallMax;

		private float fallV;
		private float fall;
		private float time;

		public Vector3 Bob(Vector3 vel)
		{
			var fll = Mathf.Clamp(vel.y * this.FallScale, -this.FallMax, 0);

			if (Mathf.Abs(vel.y) < 1)
			{
				vel.y = 0;

				this.time += vel.magnitude * 2f * Time.deltaTime;
				this.time %= Mathf.PI * 2f;
			}

			var pos = Vector3.zero;
			var cos = Mathf.Cos(this.time * 2f);

			pos.x = Mathf.Sin(this.time) * this.Offset.x;
			pos.y = -cos * this.Offset.y;
			pos.z = ((cos * 0.5f) - 0.5f) * this.Offset.z;

			this.fall = Mathf.SmoothDamp(this.fall, fll, ref this.fallV, this.FallSmoothing);

			pos.y += this.fall;

			return pos;
		}
	}
}