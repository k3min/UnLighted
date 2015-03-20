using UnityEngine;

namespace UnLighted.ImageEffects
{
	[AddComponentMenu("UnLighted/Image Effects/Bloom")]
	public class Bloom : ImageEffectBase
	{
		public int Downsample = 1;
		public float Threshold = 1;
		public float Intensity = 0.2f;

		public BloomBlur Blur = new BloomBlur
		{
			Iterations = 4,
			Size = Vector2.one
		};

		public override void OnRenderImage(RenderTexture a, RenderTexture b)
		{
			var w = a.width >> ImageEffectBase.Level(this.Downsample);
			var h = a.height >> ImageEffectBase.Level(this.Downsample);

			var rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGBHalf);

			this.Material.SetVector("_Params", new Vector2(this.Threshold, 0));

			Graphics.Blit(a, rt, this.Material, 2);

			for (var j = 0; j < this.Blur.Iterations; j++)
			{
				var rt2 = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGBHalf);
				this.Material.SetVector("_Params", new Vector2(0, this.Blur.Size.y + j));
				Graphics.Blit(rt, rt2, this.Material, 1);
				RenderTexture.ReleaseTemporary(rt);
				rt = rt2;

				rt2 = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGBHalf);
				this.Material.SetVector("_Params", new Vector2(this.Blur.Size.x + j, 0));
				Graphics.Blit(rt, rt2, this.Material, 1);
				RenderTexture.ReleaseTemporary(rt);
				rt = rt2;
			}

			this.Material.SetTexture("_Bloom", rt);
			this.Material.SetVector("_Params", new Vector2(this.Intensity, 0));

			Graphics.Blit(a, b, this.Material, 0);

			RenderTexture.ReleaseTemporary(rt);
		}
	}

	[System.Serializable]
	public struct BloomBlur
	{
		public int Iterations;
		public Vector2 Size;
	}
}