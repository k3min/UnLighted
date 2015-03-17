using UnityEngine;

namespace UnLighted.ImageEffects
{
	[RequireComponent(typeof(Camera))]
	public abstract class ImageEffectBase : MonoBehaviour
	{
		private Material material;

		public virtual string Name
		{
			get
			{
				return "Hidden/" + this.GetType().FullName.Replace(".", "-");
			}
		}

		public Shader Shader
		{
			get
			{
				return Shader.Find(this.Name);
			}
		}

		public Material Material
		{
			get
			{
				if (this.material == null)
				{
					this.material = new Material(this.Shader);
				}

				return this.material;
			}
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