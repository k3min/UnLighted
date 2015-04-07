using UnityEngine;
using System.Collections.Generic;

namespace UnLighted.ImageEffects
{
	[AddComponentMenu("UnLighted/Image Effects/Motion Blur")]
	public class MotionBlur : ImageEffectBase
	{
		private Matrix4x4 prvVP;
		private Matrix4x4 invVP;
		private Matrix4x4 VP;

		public int TargetFPS = 60;
		public int Downsample = 1;

		[HideInInspector]
		public bool Debug;

		private MotionBlurObject[] objects;

		private void Awake()
		{
			var o = new List<MotionBlurObject>();

			this.camera.depthTextureMode |= DepthTextureMode.Depth;

			foreach (var mr in Object.FindObjectsOfType<MeshRenderer>())
			{
				if (mr.gameObject.isStatic)
				{
					continue;
				}

				o.Add(mr.gameObject.AddComponent<MotionBlurObject>());
			}

			this.objects = o.ToArray();
		}

		private void UpdateTransform()
		{
			var view = this.camera.worldToCameraMatrix;
			var proj = GL.GetGPUProjectionMatrix(this.camera.projectionMatrix, false);

			this.prvVP = this.VP;
			this.VP = proj * view;
			this.invVP = this.VP.inverse;

			foreach (var o in this.objects)
			{
				o.UpdateTransform(this.VP);
			}
		}

		private void RenderVector(RenderTexture rt)
		{
			this.Material.SetMatrix("_Proj", this.prvVP * this.invVP);

			Graphics.Blit(null, rt, this.Material, 0);

			foreach (var o in this.objects)
			{
				o.RenderVector(this.Material);
			}
		}

		public override void OnRenderImage(RenderTexture a, RenderTexture b)
		{
			this.UpdateTransform();

			var w = Screen.width >> ImageEffectBase.Level(this.Downsample);
			var h = Screen.height >> ImageEffectBase.Level(this.Downsample);

			var rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.RGHalf, RenderTextureReadWrite.Linear);

			this.RenderVector(rt);

			if (this.Debug)
			{
				Graphics.Blit(rt, b);
			}
			else
			{
				this.Material.SetTexture("_Motion", rt);
				this.Material.SetFloat("_MotionScale", (1f / this.TargetFPS) / Time.deltaTime);

				Graphics.Blit(a, b, this.Material, 1);
			}

			RenderTexture.ReleaseTemporary(rt);
		}
	}
}