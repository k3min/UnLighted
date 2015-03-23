using UnityEngine;

namespace UnLighted.ImageEffects
{
	[AddComponentMenu("UnLighted/Image Effects/Flares")]
	public class Flares : ImageEffectBase
	{
		public Texture2D LensDirt;
		public Texture2D LensColor;
		public float Threshold = 1;
		public float Intensity = 0.1f;
		public float Ghost = 0.3f;
		public float Halo = 0.5f;
		public float Distortion = 1.5f;
		public int BlurIterations = 2;
		public Vector2 BlurSize = new Vector2(1, 1);
		public int Downsample;

		[HideInInspector]
		public bool Debug;

		public override void OnRenderImage(RenderTexture a, RenderTexture b)
		{
			var i = ImageEffectBase.Level(this.Downsample);

			var w = a.width >> i;
			var h = a.height >> i;

			var rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGBHalf);

			this.Common.Threshold(a, rt, this.Threshold);
			this.Common.Blur(rt, this.BlurIterations, this.BlurSize / (float)(1 << i));

			this.Material.SetTexture("_LensColor", this.LensColor);
			this.Material.SetVector("_Params", new Vector3(this.Ghost, this.Halo, this.Distortion));

			var rt2 = this.Debug ? b : RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGBHalf);

			base.OnRenderImage(rt, rt2);

			if (!this.Debug)
			{
				this.Common.Overlay(rt2, rt, Overlay.Multiply, this.LensDirt);

				RenderTexture.ReleaseTemporary(rt2);

				this.Common.Overlay(a, b, Overlay.Add, rt, this.Intensity);
			}

			RenderTexture.ReleaseTemporary(rt);
		}
	}
}