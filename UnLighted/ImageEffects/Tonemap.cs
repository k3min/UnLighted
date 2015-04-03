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
			var x = 1 << this.MipLevel;

			this.lum = new RenderTexture(x, x, 0, RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear)
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

			if (this.Debug)
			{
				Graphics.Blit(this.lum, b, this.Material, 1);
				return;
			}

			var tmp = RenderTexture.GetTemporary(4, 4, 0, RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear);

			Graphics.Blit(this.lum, tmp, this.Material, this.first ? 1 : 0);

			this.Material.SetTexture("_Adapted", tmp);
			this.Material.SetFloat("_Exposure", this.Exposure);

			Graphics.Blit(a, b, this.Material, 2);

			RenderTexture.ReleaseTemporary(tmp);

			this.first = false;
		}
	}
}
