using UnityEngine;

namespace UnLighted.ImageEffects
{
	[AddComponentMenu("UnLighted/Image Effects/Visualize")]
	public class Visualize : ImageEffectBase
	{
		public Visualize.Buffer Show;

		private void Awake()
		{
			this.camera.depthTextureMode |= DepthTextureMode.Depth;
		}

		public override void OnRenderImage(RenderTexture a, RenderTexture b)
		{
			Graphics.Blit(null, b, this.Material, (int)this.Show);
		}

		public enum Buffer
		{
			GeomertyNormals,
			GeomertyRoughness,
			GeomertySpecular,
			Depth,
			LightColor,
			LightSpecular
		}
	}
}