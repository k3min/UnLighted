using UnityEngine;

namespace UnLighted.ImageEffects
{
	[AddComponentMenu("UnLighted/Image Effects/Color Grading")]
	public class ColorGrading : ImageEffectBase
	{
		private Texture3D lut;

		public Texture2D LUT;

		public override string Name
		{
			get
			{
				return "Hidden/UnLighted/ColorGrading";
			}
		}

		public override void Awake()
		{
			if (this.LUT == null)
			{
				return;
			}

			var c2D = this.LUT.GetPixels();
			var c3D = new Color[c2D.Length];

			for (var x = 0; x < 16; x++)
			{
				for (var y = 0; y < 16; y++)
				{
					for (var z = 0; z < 16; z++)
					{
						c3D[x + (y * 16) + (z * 16 * 16)] = c2D[x + (z * 16) + (((16 - y) - 1) * 16 * 16)];
					}
				}
			}

			this.lut = new Texture3D(16, 16, 16, TextureFormat.ARGB32, false);
			this.lut.SetPixels(c3D);
			this.lut.Apply();

			base.Awake();
		}

		public override void OnRenderImage(RenderTexture a, RenderTexture b)
		{
			if (this.lut == null)
			{
				Graphics.Blit(a, b);

				return;
			}

			this.Material.SetFloat("_Scale", (16 - 1) / 16f);
			this.Material.SetFloat("_Offset", 1f / (2 * 16));
			this.Material.SetTexture("_LUT", this.lut);

			base.OnRenderImage(a, b);
		}
	}
}