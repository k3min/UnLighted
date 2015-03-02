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

		private bool Changed(Vector3 pos)
		{
			var changed = Vector3.Distance(this.prvPos, pos) > float.Epsilon;

			if (changed)
			{
				this.prvPos = pos;
			}

			return changed;
		}

		private bool Changed(Quaternion rot)
		{
			var changed = Quaternion.Angle(this.prvRot, rot) > float.Epsilon;

			if (changed)
			{
				this.prvRot = rot;
			}

			return changed;
		}

		public void UpdateTransform(Matrix4x4 vp)
		{
			var pos = this.transform.position;
			var rot = this.transform.rotation;

			this.moved = this.Changed(pos) || this.Changed(rot);

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
				if (this.renderer.sharedMaterials[i].GetTag("RenderType", true) == "Opaque" && mat.SetPass(2))
				{
					Graphics.DrawMeshNow(this.mesh, this.transform.localToWorldMatrix, i);
				}
			}
		}
	}
}