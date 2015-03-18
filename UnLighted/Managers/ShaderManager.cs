using UnityEngine;

namespace UnLighted.Managers
{
	[AddComponentMenu("UnLighted/Managers/Shader Manager")]
	public class ShaderManager : ManagerBase<ShaderManager>
	{
		public ShaderProperty[] Properties =
			{
				new ShaderProperty
				{
					Type = ShaderPropertyType.Keyword,
					Key = "REFLECTIONS",
					Enabled = true
				},
				new ShaderProperty
				{
					Type = ShaderPropertyType.Vector,
					Key = "_Shadows",
					Vector = new Vector4(0, 0.01f, 0.02f, 0.01f),
					X = "<i>Unused</i>",
					Y = "Kernel Size",
					Z = "Bias Base",
					W = "Bias Scale"
				},
				new ShaderProperty
				{
					Type = ShaderPropertyType.Float,
					Key = "_Fresnel",
					Float = 5
				}
			};

		public static void SetKeyword(string key, bool state)
		{
			var a = key + "_ON";
			var b = key + "_OFF";

			Shader.EnableKeyword((!state) ? b : a);
			Shader.DisableKeyword((!state) ? a : b);
		}

		private void Update()
		{
			foreach (var p in this.Properties)
			{
				switch (p.Type)
				{
					case ShaderPropertyType.Color:
						Shader.SetGlobalColor(p.Key, p.Color);
						break;

					case ShaderPropertyType.Float:
						Shader.SetGlobalFloat(p.Key, p.Float);
						break;

					case ShaderPropertyType.Integer:
						Shader.SetGlobalInt(p.Key, p.Integer);
						break;

					case ShaderPropertyType.Texture:
						Shader.SetGlobalTexture(p.Key, p.Texture);
						break;

					case ShaderPropertyType.Vector:
						Shader.SetGlobalVector(p.Key, p.Vector);
						break;

					case ShaderPropertyType.Keyword:
						ShaderManager.SetKeyword(p.Key, p.Enabled);
						break;
				}
			}
		}
	}

	[System.Serializable]
	public struct ShaderProperty
	{
		public ShaderPropertyType Type;
		public Vector4 Vector;
		public Texture Texture;
		public int Integer;
		public string X;
		public string Y;
		public string Z;
		public string W;
		public float Float;
		public Color Color;
		public string Key;
		public bool Enabled;
	}

	public enum ShaderPropertyType
	{
		Color,
		Float,
		Integer,
		Texture,
		Vector,
		Keyword
	}
}