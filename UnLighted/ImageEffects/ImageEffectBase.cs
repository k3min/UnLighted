using UnityEngine;

namespace UnLighted.ImageEffects
{
	[AddComponentMenu(""), RequireComponent(typeof(Camera))]
	public class ImageEffectBase : MonoBehaviour
	{
		public Material Material
		{
			get;
			set;
		}

		public virtual string Name
		{
			get
			{
				throw new System.ArgumentException();
			}
		}

		public virtual DepthTextureMode Depth
		{
			get
			{
				return DepthTextureMode.None;
			}
		}

		public virtual void Awake()
		{
			this.camera.depthTextureMode |= this.Depth;
			this.Material = new Material(Shader.Find(this.Name));
		}

		public virtual void OnRenderImage(RenderTexture a, RenderTexture b)
		{
			Graphics.Blit(a, b, this.Material);
		}

		public static int Level(int downsample)
		{
			return Mathf.Max(downsample, QualitySettings.masterTextureLimit);
		}
	}
}