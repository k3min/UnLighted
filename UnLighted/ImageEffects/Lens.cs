using UnityEngine;

namespace UnLighted.ImageEffects
{
	[AddComponentMenu("UnLighted/Image Effects/Lens")]
	public class Lens : ImageEffectBase
	{
		public float K = 0.1f;
		public float Scale = 0.91f;
		public float Blur = 2;
		public float Vignette = 0.4f;

		public override void OnRenderImage(RenderTexture a, RenderTexture b)
		{
			var p = Vector4.zero;

			p.x = this.K;
			p.y = this.Scale;
			p.z = this.Blur;
			p.w = this.Vignette;

			this.Material.SetVector("_Params", p);

			base.OnRenderImage(a, b);
		}
	}
}