using UnityEngine;

namespace UnLighted.ImageEffects
{
	[System.Serializable]
	public struct CinematicBloom
	{
		public float Spread;
		public float Threshhold;
		public float Intensity;
		public int Iterations;
		public int Downsample;
	}

	[System.Serializable]
	public struct CinematicFlare
	{
		public float Threshhold;
		public float Intensity;
		public int Iterations;
		public float Rotation;
		public float Stretch;
		public Color Color;
	}

	[AddComponentMenu("UnLighted/Image Effects/Cinematic")]
	public class Cinematic : ImageEffectBase
	{
		private const float baseSize = 1f / 512f;
		private const RenderTextureFormat format = RenderTextureFormat.ARGBHalf;

		public CinematicBloom Bloom = new CinematicBloom
		{
			Spread = 4,
			Threshhold = 1,
			Intensity = 0.2f,
			Iterations = 4,
			Downsample = 0
		};

		public CinematicFlare Flare = new CinematicFlare
		{
			Threshhold = 1,
			Intensity = 0.1f,
			Iterations = 1,
			Stretch = 4,
			Color = new Color(0.4f, 0.4f, 0.8f)
		};

		public override string Name
		{
			get
			{
				return "Hidden/UnLighted/Cinematic";
			}
		}

		public override void OnRenderImage(RenderTexture a, RenderTexture b)
		{
			var width = a.width >> this.Bloom.Downsample;
			var height = a.height >> this.Bloom.Downsample;
			var aspect = (1f * a.width) / (1f * a.height);

			var quarterRezColor = RenderTexture.GetTemporary(width, height, 0, Cinematic.format);

			Graphics.Blit(a, quarterRezColor);

			var secondQuarterRezColor = RenderTexture.GetTemporary(width, height, 0, Cinematic.format);

			this.Material.SetFloat("_Threshhold", this.Bloom.Threshhold);
			Graphics.Blit(quarterRezColor, secondQuarterRezColor, this.Material, 2);

			for (var i = 0; i < this.Bloom.Iterations; i++)
			{
				var spreadForPass = (1f + (i * 0.25f)) * this.Bloom.Spread;

				var blur4 = RenderTexture.GetTemporary(width, height, 0, Cinematic.format);
				this.Material.SetVector("_Offsets", new Vector2(0, spreadForPass * Cinematic.baseSize));
				Graphics.Blit(secondQuarterRezColor, blur4, this.Material, 0);
				RenderTexture.ReleaseTemporary(secondQuarterRezColor);
				secondQuarterRezColor = blur4;

				blur4 = RenderTexture.GetTemporary(width, height, 0, Cinematic.format);
				this.Material.SetVector("_Offsets", new Vector2((spreadForPass / aspect) * Cinematic.baseSize, 0));
				Graphics.Blit(secondQuarterRezColor, blur4, this.Material, 0);
				RenderTexture.ReleaseTemporary(secondQuarterRezColor);
				secondQuarterRezColor = blur4;
			}

			var rtFlares4 = RenderTexture.GetTemporary(width, height, 0, Cinematic.format);

			var rotation = new Vector2(Mathf.Cos(this.Flare.Rotation), Mathf.Sin(this.Flare.Rotation));
			var stretch = (this.Flare.Stretch * 1f / aspect) * Cinematic.baseSize;

			this.Material.SetVector("_Offsets", rotation);
			this.Material.SetFloat("_Threshhold", this.Flare.Threshhold);
			this.Material.SetVector("_Color", this.Flare.Color);
			this.Material.SetFloat("_Intensity", this.Flare.Intensity);

			quarterRezColor.DiscardContents();
			Graphics.Blit(rtFlares4, quarterRezColor, this.Material, 3);

			rtFlares4.DiscardContents();
			Graphics.Blit(quarterRezColor, rtFlares4, this.Material, 4);

			this.Material.SetVector("_Offsets", rotation * stretch);

			this.Material.SetFloat("_Stretch", this.Flare.Stretch);
			quarterRezColor.DiscardContents();
			Graphics.Blit(rtFlares4, quarterRezColor, this.Material, 5);

			this.Material.SetFloat("_Stretch", this.Flare.Stretch * 2f);
			rtFlares4.DiscardContents();
			Graphics.Blit(quarterRezColor, rtFlares4, this.Material, 5);

			this.Material.SetFloat("_Stretch", this.Flare.Stretch * 4f);
			quarterRezColor.DiscardContents();
			Graphics.Blit(rtFlares4, quarterRezColor, this.Material, 5);

			for (var u = 0; u < this.Flare.Iterations; u++)
			{
				stretch = (this.Flare.Stretch * 2f / aspect) * Cinematic.baseSize;

				this.Material.SetVector("_Offsets", rotation * stretch);
				rtFlares4.DiscardContents();
				Graphics.Blit(quarterRezColor, rtFlares4, this.Material, 0);

				this.Material.SetVector("_Offsets", rotation * stretch);
				quarterRezColor.DiscardContents();
				Graphics.Blit(rtFlares4, quarterRezColor, this.Material, 0);
			}

			secondQuarterRezColor.MarkRestoreExpected();
			Graphics.Blit(quarterRezColor, secondQuarterRezColor, this.Material, 6);

			RenderTexture.ReleaseTemporary(rtFlares4);

			this.Material.SetFloat("_Intensity", this.Bloom.Intensity);
			this.Material.SetTexture("_Bloom", secondQuarterRezColor);

			Graphics.Blit(a, b, this.Material, 1);

			RenderTexture.ReleaseTemporary(quarterRezColor);
			RenderTexture.ReleaseTemporary(secondQuarterRezColor);
		}
	}
}