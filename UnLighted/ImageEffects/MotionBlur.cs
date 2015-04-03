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

		private RenderTexture motion;
		private List<MotionBlurObject> objects = new List<MotionBlurObject>();

		private void Awake()
		{
			this.camera.depthTextureMode |= DepthTextureMode.Depth;

			var w = Screen.width >> ImageEffectBase.Level(this.Downsample);
			var h = Screen.height >> ImageEffectBase.Level(this.Downsample);

			this.motion = new RenderTexture(w, h, 0, RenderTextureFormat.RGHalf, RenderTextureReadWrite.Linear);

			foreach (var mr in Object.FindObjectsOfType<MeshRenderer>())
			{
				if (mr.gameObject.isStatic)
				{
					continue;
				}

				this.objects.Add(mr.gameObject.AddComponent<MotionBlurObject>());
			}
		}

		private void UpdateTransform()
		{
			var view = this.camera.worldToCameraMatrix;
			var proj = GL.GetGPUProjectionMatrix(this.camera.projectionMatrix, false);

			this.prvVP = this.VP;
			this.VP = proj * view;
			this.invVP = this.VP.inverse;

			this.objects.ForEach(o => o.UpdateTransform(this.VP));
		}

		private void RenderVector()
		{
			this.Material.SetMatrix("_Proj", this.prvVP * this.invVP);

			Graphics.Blit(null, this.motion, this.Material, 0);

			var rt = RenderTexture.active;

			RenderTexture.active = this.motion;

			this.objects.ForEach(o => o.RenderVector(this.Material));

			RenderTexture.active = rt;
		}

		public override void OnRenderImage(RenderTexture a, RenderTexture b)
		{
			this.UpdateTransform();
			this.RenderVector();

			if (this.Debug)
			{
				Graphics.Blit(this.motion, b);
				return;
			}

			this.Material.SetTexture("_MotionTex", this.motion);
			this.Material.SetFloat("_MotionScale", (1f / this.TargetFPS) / Time.deltaTime);

			Graphics.Blit(a, b, this.Material, 1);
		}
	}
}