using UnityEditor;
using UnityEngine;

public class CustomAssetPostProcessor : AssetPostprocessor
{
    void OnPreprocessAsset()
    {
        if (assetImporter.assetPath.Contains("Videos"))
        {
            var texImporter = assetImporter as TextureImporter;
            if (texImporter != null)
            {
                texImporter.textureType = TextureImporterType.Default;
            }
        }
    }
}
