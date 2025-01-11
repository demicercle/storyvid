using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorTools 
{
    [MenuItem("Tools/Save Selected Assets Thumbnails")]
    static public void SaveSelectedAssetsThumbnails()
    {
        var savePath = EditorUtility.SaveFolderPanel("Save Selected Assets Thumbnails", "Assets", "");
        if (string.IsNullOrEmpty(savePath))
            return;
        
        foreach (Object obj in Selection.objects)
        {
            var thumbnail = AssetPreview.GetMiniThumbnail(obj);
            if (thumbnail != null)
            {
                var bytes = thumbnail.EncodeToPNG();
                var assetPath = savePath + "/" + obj.name + ".png";
                System.IO.File.WriteAllBytes(assetPath, bytes);
                Debug.Log("Created png at " + assetPath);
            }
        }
        
        AssetDatabase.SaveAssets();
    }
}
