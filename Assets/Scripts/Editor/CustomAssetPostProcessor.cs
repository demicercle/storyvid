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
        
        if (assetImporter is VideoClipImporter videoImporter)
        {
            // Vérifie que le chemin contient "/Resources/"
            if (assetPath.Contains("/Resources/"))
            {
                // Liste des plateformes cibles
                string[] platforms = new string[] { "Standalone", "Android", "iOS" };

                foreach (string platform in platforms)
                {
                    VideoImporterTargetSettings settings = videoImporter.GetTargetSettings(platform);

                    if (!settings.enableTranscoding)
                    {
                        settings.enableTranscoding = true;
                        videoImporter.SetTargetSettings(platform, settings);
                        Debug.Log($"[VideoClipTranscodeProcessor] Transcode activé pour la plateforme {platform} : {assetPath}");
                    }
                }
            }
        }
    }
}
