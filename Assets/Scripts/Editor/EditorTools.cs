using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorTools 
{
    
    [MenuItem("Tools/Open Persistent Data Path")]
    static void OpenPersistentDataPath()
    {
        string path = Application.persistentDataPath;

#if UNITY_EDITOR_OSX
        System.Diagnostics.Process.Start("open", $"\"{path}\"");
#else
        Application.OpenURL("file://" + path);
#endif
    }
    
    [MenuItem("Tools/Clear PlayerPrefs")]
    static public void ClearPlayerPrefs()
    {
        if (EditorUtility.DisplayDialog("Clear Player Prefs", "Are you sure?", "Yes", "No"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("Player Prefs deleted");
        }
    }
    
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
        AssetDatabase.Refresh();
    }
}
