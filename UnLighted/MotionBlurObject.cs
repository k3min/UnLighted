using UnityEngine;

namespace UnLighted
{
	[AddComponentMenu(""), RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class MotionBlurObject : MonoBehaviour
	{
		private Vector3 prvPos;
		private Quaternion prvRot;
		private Matrix4x4 prvMVP;
		private Matrix4x4 MVP;
		private Mesh mesh;
		private bool moved;

		private void Awake()
		{
			this.mesh = this.GetComponent<MeshFilter>().mesh;
		}

		private static bool Changed(Vector3 cur, Vector3 prv)
		{
			return Vector3.SqrMagnitude(cur - prv) > float.Epsilon;
		}

		private static bool Changed(Quaternion cur, Quaternion prv)
		{
			return Quaternion.Angle(cur, prv) > float.Epsilon;
		}

		public void UpdateTransform(Matrix4x4 vp)
		{
			var pos = this.transform.position;
			var rot = this.transform.rotation;

			this.moved = MotionBlurObject.Changed(pos, this.prvPos) || MotionBlurObject.Changed(rot, this.prvRot);

			if (this.moved)
			{
				this.prvPos = pos;
				this.prvRot = rot;
			}

			this.prvMVP = this.MVP;

			this.MVP = vp * this.transform.localToWorldMatrix;
		}

		public void RenderVectors(Material mat)
		{
			if (!this.renderer.isVisible || !this.moved)
			{
				return;
			}

			mat.SetMatrix("_Proj", this.prvMVP);

			for (int i = 0; i < this.renderer.sharedMaterials.Length; i++)
			{
				if (mat.SetPass(2))
				{
					Graphics.DrawMeshNow(this.mesh, this.transform.localToWorldMatrix, i);
				}
			}
		}
	}
}