using UnityEngine;

namespace UnLighted.ImageEffects
{
	[RequireComponent(typeof(Camera))]
	public abstract class ImageEffectBase : MonoBehaviour
	{
		private Material material;
		private Common common;

		public virtual string Name
		{
			get
			{
				return "Hidden/" + this.GetType().FullName.Replace(".", "-");
			}
		}

		public Material Material
		{
			get
			{
				if (this.material == null)
				{
					this.material = new Material(Shader.Find(this.Name));
				}

				return this.material;
			}
		}

		public Common Common
		{
			get
			{
				if (this.common == null)
				{
					this.common = this.Get<Common>();
					this.common.hideFlags = HideFlags.HideInInspector;
					this.common.enabled = false;
				}

				return this.common;
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