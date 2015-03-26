using UnityEditor;
using UnityEngine;
using UnLightedEd.ImageEffects;
using UnLighted.ImageEffects;

// No namespaces for custom material editors
internal class UberTranslucencyMaterialEditor : MaterialEditor
{
	public override void OnInspectorGUI()
	{
		if (Object.FindObjectOfType<Thickness>() == null)
		{
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.HelpBox("This shader needs the Thickness ImageEffect!", MessageType.Warning);

				if (GUILayout.Button("Fix", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true)))
				{
					ThicknessEditor.Hide(new GameObject("Thickness").AddComponent<Thickness>());
				}
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Separator();
		}

		base.OnInspectorGUI();
	}
}