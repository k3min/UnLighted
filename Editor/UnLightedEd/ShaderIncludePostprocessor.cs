using UnityEditor;
using System.IO;
using System.Linq;

namespace UnLightedEd
{
	// Analysis disable once ConvertToStaticType
	internal class ShaderIncludePostprocessor : AssetPostprocessor
	{
		private static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom)
		{
			foreach (var path in imported)
			{
				if (Path.GetExtension(path) != ".cginc")
				{
					continue;
				}

				ShaderIncludePostprocessor.ReimportDependents(path);
			}
		}

		private static void ReimportDependents(string path)
		{
			var guids = AssetDatabase.FindAssets("t:shader");
			var fileName = Path.GetFileName(path);

			foreach (var asset in guids.Select(AssetDatabase.GUIDToAssetPath))
			{
				using (var stream = new StreamReader(asset))
				{
					if (stream.ReadToEnd().Contains(fileName))
					{
						AssetDatabase.ImportAsset(asset);
					}
				}
			}
		}
	}
}