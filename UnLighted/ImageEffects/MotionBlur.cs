using UnityEngine;
using UnLighted.ImageEffects;
using System.Collections.Generic;

namespace UnLighted.ImageEffects
{
	[AddComponentMenu("UnLighted/Image Effects/Motion Blur")]
	public class MotionBlur : ImageEffectBase
	{
		private Matrix4x4 prvVP;
		private Matrix4x4 invVP;
		private Matrix4x4 VP;

		public float TargetFPS = 60;
		public int MaxSamples = 32;
		public int Downsample = 1;
		public bool Debug;

		private RenderTexture motion;

		private List<MotionBlurObject> objects = new List<MotionBlurObject>();

		public override string Name
		{
			get
			{
				return "Hidden/UnLighted/MotionBlur";
			}
		}

		public override DepthTextureMode Depth
		{
			get
			{
				return DepthTextureMode.Depth;
			}
		}

		public override void Awake()
		{
			base.Awake();

			var w = Screen.width >> ImageEffectBase.Level(this.Downsample);
			var h = Screen.height >> ImageEffectBase.Level(this.Downsample);

			this.motion = new RenderTexture(w, h, 0, RenderTextureFormat.ARGBHalf);

			foreach (var mr in Object.FindObjectsOfType<MeshRenderer>())
			{
				if (mr.gameObject.isStatic)
				{
					continue;
				}

				this.objects.Add(mr.gameObject.AddComponent<MotionBlurObject>());
			}
		}

		private void OnPreRender()
		{
			var view = this.camera.worldToCameraMatrix;
			var proj = GL.GetGPUProjectionMatrix(this.camera.projectionMatrix, false);

			this.prvVP = this.VP;
			this.VP = proj * view;
			this.invVP = this.VP.inverse;

			this.objects.ForEach(o => o.UpdateTransform(this.VP));
		}

		private void OnPostRender()
		{
			this.Material.SetMatrix("_Proj", this.prvVP * this.invVP);

			Graphics.Blit(null, this.motion, this.Material, 0);

			var rt = RenderTexture.active;

			Graphics.SetRenderTarget(this.motion);

			this.objects.ForEach(o => o.RenderVectors(this.Material));

			Graphics.SetRenderTarget(rt);
		}

		public override void OnRenderImage(RenderTexture a, RenderTexture b)
		{
			this.Material.SetTexture("_MotionTex", this.motion);
			this.Material.SetFloat("_MotionScale", (1f / this.TargetFPS) / Time.deltaTime);
			this.Material.SetInt("_MaxSamples", this.MaxSamples);

			Graphics.Blit(a, b, this.Material, 1);
		}

		private void OnGUI()
		{
			if (!this.Debug)
			{
				return;
			}

			GUI.DrawTexture(new Rect(0, 0, this.motion.width, this.motion.height), this.motion, ScaleMode.ScaleAndCrop, false);
		}
	}
}