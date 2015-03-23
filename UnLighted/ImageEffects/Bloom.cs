using UnityEngine;

namespace UnLighted.ImageEffects
{
	[AddComponentMenu("UnLighted/Image Effects/Bloom")]
	public class Bloom : ImageEffectBase
	{
		public int Downsample = 1;
		public float Threshold = 1;
		public float Intensity = 0.2f;
		public int BlurIterations = 4;
		public Vector2 BlurSize = new Vector2(2, 2);

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

			if (this.Debug)
			{
				Graphics.Blit(rt, b);
			}
			else
			{
				this.Common.Overlay(a, b, Overlay.Add, rt, this.Intensity);
			}

			RenderTexture.ReleaseTemporary(rt);
		}
	}
}