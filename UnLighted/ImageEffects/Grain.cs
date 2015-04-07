using UnityEngine;

namespace UnLighted.ImageEffects
{
	[AddComponentMenu("UnLighted/Image Effects/Grain")]
	public class Grain : ImageEffectBase
	{
		public float Amount = 0.05f;
		public float Threshold = 0.2f;

		[HideInInspector]
		public bool Debug;

		public override void OnRenderImage(RenderTexture a, RenderTexture b)
		{
			var p = Vector2.zero;

			p.x = this.Amount;
			p.y = this.Threshold;

			this.Material.SetVector("_Params", p);

			Graphics.Blit(a, b, this.Material, this.Debug ? 1 : 0);
		}
	}
}