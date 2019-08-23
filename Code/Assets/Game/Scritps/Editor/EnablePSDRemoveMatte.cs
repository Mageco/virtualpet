using UnityEngine;
using UnityEditor;

internal class EnablePSDRemoveMatte : AssetPostprocessor
{
	private void OnPreprocessTexture()
	{
		if (assetPath.Contains(".psd"))
		{
			TextureImporter textureImporter = (TextureImporter)assetImporter;

			var serializedObject = new SerializedObject(textureImporter);
			serializedObject.FindProperty("m_PSDRemoveMatte").boolValue = true;
			serializedObject.FindProperty("m_PSDShowRemoveMatteOption").boolValue = true; // this is not needed unless you want to show the option (and warning)
			serializedObject.ApplyModifiedProperties();
		}
	}
}