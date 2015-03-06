﻿using UnityEngine;

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
		private Material[] materials;

		public bool Fixed { get { return this.rigidbody != null; } }

		private void Awake()
		{
			this.mesh = this.GetComponent<MeshFilter>().mesh;
			this.materials = this.renderer.sharedMaterials;
		}

		public void UpdateTransform(Matrix4x4 vp)
		{
			var pos = Vector3.Distance(this.prvPos, this.transform.position) > float.Epsilon;
			var rot = Quaternion.Angle(this.prvRot, this.transform.rotation) > float.Epsilon;

			if (pos)
			{
				this.prvPos = this.transform.position;
			}

			if (rot)
			{
				this.prvRot = this.transform.rotation;
			}

			this.moved = pos || rot;

			this.prvMVP = this.MVP;
			this.MVP = vp * this.transform.localToWorldMatrix;
		}

		public void RenderVectors(Material material)
		{
			if (!this.renderer.isVisible || !this.moved)
			{
				return;
			}

			var matrix = this.transform.localToWorldMatrix;

			material.SetMatrix("_Proj", this.prvMVP);

			for (var i = 0; i < this.materials.Length; i++)
			{
				var type = this.materials[i].GetTag("RenderType", true);

				if (type == "Opaque" && material.SetPass(2))
				{
					Graphics.DrawMeshNow(this.mesh, matrix, i);
				}
			}
		}
	}
}