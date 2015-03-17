using UnityEngine;

namespace UnLighted.ImageEffects
{
	[AddComponentMenu("UnLighted/Image Effects/Lens")]
	public class Lens : ImageEffectBase
	{
		public float K = 0.1f;
		public float Scale = 0.95f;
		public float Dispersion = 0.01f;
		public Vector3 ETA = new Vector3(0.9f, 0.6f, 0.3f);

		public override void OnRenderImage(RenderTexture a, RenderTexture b)
		{
			this.Material.SetVector("_ETA", this.ETA * this.Dispersion);
			this.Material.SetVector("_Params", new Vector2(this.K, this.Scale));

			base.OnRenderImage(a, b);
		}
	}
}