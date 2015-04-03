using UnityEngine;

namespace UnLighted.ImageEffects
{
	[AddComponentMenu("UnLighted/Image Effects/Thickness")]
	public class Thickness : ImageEffectBase
	{
		private RenderTexture thickness;

		public int Downsample;

		private void Awake()
		{
			var w = Screen.width >> ImageEffectBase.Level(this.Downsample);
			var h = Screen.height >> ImageEffectBase.Level(this.Downsample);

			this.thickness = new RenderTexture(w, h, 0, RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear);

			Shader.SetGlobalTexture("_Thickness", this.thickness);
		}

		private void OnPreCull()
		{
			this.camera.CopyFrom(Camera.main);

			this.camera.clearFlags = CameraClearFlags.Color;
			this.camera.backgroundColor = new Color(0, 0, 0, 0);
			this.camera.depthTextureMode |= DepthTextureMode.DepthNormals;
			this.camera.cullingMask = -1;
			this.camera.hdr = true;
			this.camera.renderingPath = RenderingPath.Forward;
			this.camera.targetTexture = this.thickness;

			this.camera.SetReplacementShader(this.Material.shader, "RenderType");
		}
	}
}