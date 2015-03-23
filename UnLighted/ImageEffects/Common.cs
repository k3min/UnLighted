using UnityEngine;

namespace UnLighted.ImageEffects
{
	[AddComponentMenu("")]
	public class Common : ImageEffectBase
	{
		public void Overlay(RenderTexture a, RenderTexture b, Overlay o, Texture t, float f = 1)
		{
			this.Material.SetTexture("_Overlay", t);
			this.Material.SetFloat("_Opacity", f);

			Graphics.Blit(a, b, this.Material, (int)o);
		}

		public void Blur(RenderTexture rt, int iterations, Vector2 size)
		{
			var rt2 = RenderTexture.GetTemporary(rt.width, rt.height, 0, RenderTextureFormat.ARGBHalf);

			for (var i = 0; i < iterations; i++)
			{
				this.Material.SetVector("_Size", new Vector2(0, size.y + i));
				Graphics.Blit(rt, rt2, this.Material, 1);

				this.Material.SetVector("_Size", new Vector2(size.x + i, 0));
				Graphics.Blit(rt2, rt, this.Material, 1);
			}

			RenderTexture.ReleaseTemporary(rt2);
		}

		public void Threshold(RenderTexture a, RenderTexture b, float threshold)
		{
			this.Material.SetFloat("_Threshold", threshold);

			Graphics.Blit(a, b, this.Material, 2);
		}
	}

	public enum Overlay
	{
		Add = 0,
		Multiply = 3
	}
}