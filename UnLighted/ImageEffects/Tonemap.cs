using UnityEngine;

namespace UnLighted.ImageEffects
{
	[AddComponentMenu("UnLighted/Image Effects/Tonemap")]
	public class Tonemap : ImageEffectBase
	{
		private bool first = true;
		private RenderTexture lum;

		public int MipLevel = 8;
		public float AdaptionRate = 2.5f;
		public float Exposure = 1;

		[HideInInspector]
		public bool Debug;

		private void Awake()
		{
			this.lum = new RenderTexture(1 << this.MipLevel, 1 << this.MipLevel, 0, RenderTextureFormat.ARGBHalf)
			{
				useMipMap = true
			};
		}

		[ImageEffectTransformsToLDR]
		public override void OnRenderImage(RenderTexture a, RenderTexture b)
		{
			Graphics.Blit(a, this.lum);

			this.Material.SetFloat("_AdaptionRate", this.AdaptionRate);
			this.Material.SetInt("_Level", this.MipLevel);

			var tmp = RenderTexture.GetTemporary(1, 1, 0, RenderTextureFormat.ARGBHalf);

			Graphics.Blit(this.lum, this.Debug ? b : tmp, this.Material, (this.first || this.Debug) ? 1 : 0);

			if (!this.Debug)
			{
				this.Material.SetTexture("_Adapted", tmp);
				this.Material.SetFloat("_Exposure", this.Exposure);

				Graphics.Blit(a, b, this.Material, 2);
			}

			RenderTexture.ReleaseTemporary(tmp);

			this.first = false;
		}
	}
}
