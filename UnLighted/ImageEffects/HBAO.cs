using UnityEngine;

namespace UnLighted.ImageEffects
{
	[AddComponentMenu("UnLighted/Image Effects/HBAO")]
	public class HBAO : ImageEffectBase
	{
		public float Radius = 0.3f;
		public float Intensity = 5;
		public int Downsample;
		public Texture2D Random;

		public override string Name
		{
			get
			{
				return "Hidden/UnLighted/HBAO";
			}
		}

		public override void OnRenderImage(RenderTexture a, RenderTexture b)
		{
			var width = a.width >> ImageEffectBase.Level(this.Downsample);
			var height = a.height >> ImageEffectBase.Level(this.Downsample);

			var p = this.camera.projectionMatrix;
			var projInfo = new Vector4(
				               -2f / (a.width * p[0]),
				               -2f / (a.height * p[5]),
				               (1f - p[2]) / p[0],
				               (1f + p[6]) / p[5]
			               );

			this.Material.SetFloat("_Radius", this.Radius);
			this.Material.SetFloat("_Intensity", this.Intensity);
			this.Material.SetTexture("_Random", this.Random);
			this.Material.SetMatrix("_Proj", p);
			this.Material.SetVector("_ProjInfo", projInfo);

			var rt = RenderTexture.GetTemporary(width, height, 0);

			Graphics.Blit(a, rt, this.Material, 0);

			this.Material.SetTexture("_AO", rt);

			Graphics.Blit(a, b, this.Material, 1);

			RenderTexture.ReleaseTemporary(rt);
		}
	}
}

