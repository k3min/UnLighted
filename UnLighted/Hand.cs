using UnityEngine;
using UnLighted.Controllers;

namespace UnLighted
{
	[AddComponentMenu("UnLighted/Hand")]
	public class Hand : MonoBehaviour
	{
		private Joint joint;
		private RaycastHit hit;
		private float size;
		private Material material;

		public float MaxDistance = 2;
		public float Break = 1000;
		public Pointer Pointer = new Pointer
		{
			Min = 2,
			Max = 8,
			Color = new Color(0, 0, 0, 0.5f),
			Segments = 16,
			Thickness = 3,
			Speed = 10
		};

		public bool Valid
		{
			get
			{
				return this.hit.rigidbody != null && !this.hit.rigidbody.isKinematic;
			}
		}

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

		private void Awake()
		{
			this.material = new Material(Shader.Find("Hidden/UnLighted-ImageEffects-Pointer"));
		}

		private void Update()
		{
			var origin = Camera.main.transform.position;
			var direction = Camera.main.transform.forward;

			Physics.Raycast(origin, direction, out this.hit, this.MaxDistance);

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

			if (!this.Valid)
			{
				return;
			}

			if (this.joint == null)
			{
				this.joint = this.gameObject.AddComponent<FixedJoint>();
				this.joint.breakTorque = this.Break;
				this.joint.hideFlags = HideFlags.HideInInspector;

				this.rigidbody.isKinematic = true;
				this.rigidbody.useGravity = false;
				this.rigidbody.solverIterationCount = PlayerController.SolverCount;
				this.rigidbody.hideFlags = HideFlags.HideInInspector;
			}

			this.Holding = this.hit.rigidbody;
			this.Holding.solverIterationCount = PlayerController.SolverCount;
		}

		private Vector3 Vertex(float i, float o)
		{
			var v = Vector3.zero;
			var f = ((float)i / this.Pointer.Segments) * Mathf.PI * 2f;
			var s = this.size + (o * this.Pointer.Thickness);

			v.x = ((Mathf.Cos(f) / Screen.width) * s) + 0.5f;
			v.y = ((Mathf.Sin(f) / Screen.height) * s) + 0.5f;

			return v;
		}

		private void OnGUI()
		{
			var s = (this.Holding != null || this.Valid) ? this.Pointer.Max : this.Pointer.Min;

			this.size = Mathf.Lerp(this.size, s, this.Pointer.Speed * Time.deltaTime);

			GL.PushMatrix();
			GL.LoadOrtho();

			this.material.SetPass(0);
			this.material.SetColor("_Color", this.Pointer.Color);

			GL.Begin(GL.QUADS);

			for (var i = 0; i < this.Pointer.Segments; i++)
			{
				GL.TexCoord2(0, 0);
				GL.Vertex(this.Vertex(i, 0));

				GL.TexCoord2(0, 0);
				GL.Vertex(this.Vertex(i + 1, 0));

				GL.TexCoord2(0, 1);
				GL.Vertex(this.Vertex(i + 1, 1));

				GL.TexCoord2(0, 1);
				GL.Vertex(this.Vertex(i, 1));
			}

			GL.End();

			GL.PopMatrix();
		}
	}

	[System.Serializable]
	public struct Pointer
	{
		public float Min;
		public float Max;
		public Color Color;
		public int Segments;
		public float Thickness;
		public float Speed;
	}
}