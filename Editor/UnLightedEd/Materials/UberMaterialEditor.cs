using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace UnLightedEd.Materials
{
	internal class UberMaterialEditor : MaterialEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var mat = base.target as Material;
			var kys = new List<string>();

			foreach (var p in MaterialEditor.GetMaterialProperties(base.targets))
			{
				var tex = (p.type == MaterialProperty.PropType.Texture);
				var hid = (p.flags == MaterialProperty.PropFlags.HideInInspector);

				if (!tex || hid)
				{
					continue;
				}

				kys.Add(p.displayName.ToUpper() + ((p.textureValue != null) ? "_ON" : "_OFF"));
			}

			mat.shaderKeywords = kys.ToArray();

			if (GUI.changed)
			{
				EditorUtility.SetDirty(mat);
			}
		}
	}
}