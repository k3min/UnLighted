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
		public bool Debug;

		public override string Name
		{
			get
			{
				return "Hidden/UnLighted/Tonemap";
			}
		}

		public override void Awake()
		{
			base.Awake();

			this.lum = new RenderTexture(1 << this.MipLevel, 1 << this.MipLevel, 0, RenderTextureFormat.ARGBHalf)
			{
				useMipMap = true
			};
		}

		[ImageEffectTransformsToLDR]
		public override void OnRenderImage(RenderTexture a, RenderTexture b)
		{
			Graphics.Blit(a, this.lum);

			base.Material.SetFloat("_AdaptionRate", this.AdaptionRate);
			base.Material.SetInt("_Level", this.MipLevel);

			var tmp = RenderTexture.GetTemporary(1, 1, 0, RenderTextureFormat.ARGBHalf);

			Graphics.Blit(this.lum, this.Debug ? b : tmp, base.Material, (this.first || this.Debug) ? 1 : 0);

			if (!this.Debug)
			{
				base.Material.SetTexture("_Adapted", tmp);
				base.Material.SetFloat("_Exposure", this.Exposure);

				Graphics.Blit(a, b, base.Material, 2);
			}

			RenderTexture.ReleaseTemporary(tmp);

			this.first = false;
		}
	}
}
