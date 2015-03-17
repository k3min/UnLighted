using UnityEngine;

namespace UnLighted.ImageEffects
{
	[AddComponentMenu("UnLighted/Image Effects/Bloom")]
	public class Bloom : ImageEffectBase
	{
		public int Downsample = 1;
		public int Iterations = 4;
		public float BlurSize = 4;
		public float Intensity = 0.2f;

		public override void OnRenderImage(RenderTexture a, RenderTexture b)
		{
			var w = a.width >> ImageEffectBase.Level(this.Downsample);
			var h = a.height >> ImageEffectBase.Level(this.Downsample);

			var rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGBHalf);

			Graphics.Blit(a, rt);

			for (var j = 0; j < this.Iterations; j++)
			{
				var rt2 = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGBHalf);
				this.Material.SetVector("_BlurSize", new Vector2(0, this.BlurSize + j));
				Graphics.Blit(rt, rt2, this.Material, 1);
				RenderTexture.ReleaseTemporary(rt);
				rt = rt2;

				rt2 = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGBHalf);
				this.Material.SetVector("_BlurSize", new Vector2(this.BlurSize + j, 0));
				Graphics.Blit(rt, rt2, this.Material, 1);
				RenderTexture.ReleaseTemporary(rt);
				rt = rt2;
			}

			this.Material.SetTexture("_Bloom", rt);
			this.Material.SetFloat("_Intensity", this.Intensity);

			Graphics.Blit(a, b, this.Material, 0);

			RenderTexture.ReleaseTemporary(rt);
		}
	}
}