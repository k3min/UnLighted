using UnityEngine;
using UnLighted.Managers;
using System.IO;
using System.Collections.Generic;

namespace UnLighted.ImageEffects
{
	[AddComponentMenu("")]
	public class BoxProbe : ImageEffectBase
	{
		private const int size = 256;
		private const string path = "UnLighted/Editor/Box";

		private readonly KeyValuePair<CubemapFace, Vector3>[] faces =
			{
				new KeyValuePair<CubemapFace, Vector3>(CubemapFace.NegativeX, new Vector3(0, 270)),
				new KeyValuePair<CubemapFace, Vector3>(CubemapFace.NegativeY, new Vector3(90, 0)),
				new KeyValuePair<CubemapFace, Vector3>(CubemapFace.NegativeZ, new Vector3(0, 180)),
				new KeyValuePair<CubemapFace, Vector3>(CubemapFace.PositiveX, new Vector3(0, 90)),
				new KeyValuePair<CubemapFace, Vector3>(CubemapFace.PositiveY, new Vector3(270, 0)),
				new KeyValuePair<CubemapFace, Vector3>(CubemapFace.PositiveZ, new Vector3(0, 0))
			};

		private Texture2D tex;
		private int i;

		public override string Name
		{
			get
			{
				return "Hidden/UnLighted/BoxProbe";
			}
		}

		public override void Awake()
		{
			base.Awake();

			this.camera.aspect = 1;
			this.camera.fieldOfView = 90;
			this.camera.nearClipPlane = 0.1f;
			this.camera.farClipPlane = 100;
			this.camera.hdr = true;
			this.camera.clearFlags = CameraClearFlags.Depth;
			this.camera.cullingMask = 1;

			this.tex = new Texture2D(BoxProbe.size, BoxProbe.size, TextureFormat.ARGB32, false, true);

			var p = string.Format(
				        "{0}/{1}/{2}",
				        Application.dataPath,
				        BoxProbe.path,
				        this.gameObject.name
			        );

			Directory.CreateDirectory(p);
		}

		private void Update()
		{
			if (this.i >= 6)
			{
				return;
			}

			this.transform.eulerAngles = this.faces[this.i].Value;
		}

		public override void OnRenderImage(RenderTexture a, RenderTexture b)
		{
			if (this.i >= 6)
			{
				return;
			}

			var rt = RenderTexture.GetTemporary(BoxProbe.size, BoxProbe.size, 0, RenderTextureFormat.ARGB32);

			base.OnRenderImage(a, rt);

			this.tex.ReadPixels(new Rect(0, 0, BoxProbe.size, BoxProbe.size), 0, 0);
			this.tex.Apply();

			RenderTexture.ReleaseTemporary(rt);

			var p = string.Format(
				        "{0}/{1}/{2}/{3}.png",
				        Application.dataPath,
				        BoxProbe.path,
				        this.gameObject.name,
				        System.Enum.GetName(typeof(CubemapFace), this.faces[this.i].Key)
			        );

			File.WriteAllBytes(p, this.tex.EncodeToPNG());

			Graphics.Blit(a, b);

			this.i += 1;
		}

		private void LateUpdate()
		{
			if (this.i < 6)
			{
				return;
			}

			Object.Destroy(this.gameObject);
		}

		private void OnDestroy()
		{
			BoxProbe.Others(true);
		}

		public static void Render(Box box)
		{
			BoxProbe.Others(false);

			var go = new GameObject(box.gameObject.name, typeof(BoxProbe));

			go.transform.position = box.transform.position;
		}

		private static void Others(bool restore)
		{
			var cam = GameObject.FindWithTag("MainCamera");

			if (cam != null)
			{
				cam.camera.enabled = restore;
			}

			foreach (var ren in Object.FindObjectsOfType<Renderer>())
			{
				if (ren.gameObject.isStatic)
				{
					continue;
				}

				ren.enabled = restore;
			}

			foreach (var lig in Object.FindObjectsOfType<Light>())
			{
				if (lig.alreadyLightmapped)
				{
					continue;
				}

				lig.enabled = restore;
			}

			var cubemap = restore ? BoxManager.Main.Box.Cubemap : new Cubemap(4, TextureFormat.ARGB32, false);

			Shader.SetGlobalTexture("_Box", cubemap);
		}
	}
}