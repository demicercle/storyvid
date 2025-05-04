using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SavedGame
{
    static string filePath => Application.persistentDataPath + "/SavedGame.dat";

    public void DeleteSave()
    {
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
            Debug.Log(this + " save file deleted");
        }
    }
    
    public void Load()
    {
        if (System.IO.File.Exists(filePath))
        {
            var cryptedJson = System.IO.File.ReadAllText(filePath);
            var json = EncryptionService.DecryptWithDeviceId<string>(cryptedJson);
            Debug.Log(this + " Load: " + json);
            JsonUtility.FromJsonOverwrite(json, this);
        }
        else
        {
            Debug.Log(this + " Load: no file found");
        }
    }

    public void Save()
    {
        var json = JsonUtility.ToJson(this);
        Debug.Log(this + " Save: " + json);
        var cryptedJson = EncryptionService.EncryptWithDeviceId(json);
        System.IO.File.WriteAllText(filePath, cryptedJson);
        changed = false;
    }

    public bool IsPathUnlocked(int episode, string videoID)
    {
        var key = episode + "." + videoID;
        return unlocks.Contains(key);
    }
    
    public void UnlockPath(int episode, string videoID)
    {
        var key = episode + "." + videoID;
        if (!unlocks.Contains(key))
        {
            unlocks.Add(key);
            changed = true;
        }
    }

    public bool IsLinkVisited(int link) => links.Contains(link);

    public void SetLinkVisited(int link, bool value)
    {
        if (value && !links.Contains(link))
            links.Add(link);
        else if (!value && links.Contains(link))
            links.Remove(link);
        changed = true;
    }
    
    public bool IsEpisodeCompleted(int episode)
    {
        return episodes.Contains(episode);
    }

    public void SetEpisodeCompleted(int episode, bool value)
    {
        if (value && !episodes.Contains(episode))
            episodes.Add(episode);
        else if (!value && episodes.Contains(episode))
            episodes.Remove(episode);
        changed = true;
    }

    public int GetPoints()
    {
        var count = 0;
        foreach (var link in GameManager.instance.videoLinks)
        {
            if (links.Contains(link.id))
            {
                count += link.points;
            }
        }
        return count;
    }
    
    public bool IsEpisodeUnlocked(int episode)
    {
        if (episode == 5)
        {
            return (IsEpisodeCompleted(3) && GetPoints() >= 3) || IsEpisodeCompleted(4);
        }
        else if (episode == 4)
        {
            return (IsEpisodeCompleted(3) && GetPoints() < 3) || IsEpisodeCompleted(5);
        }
        else
        {
            return true;
        }
    }

    public List<string> unlocks = new List<string>();
    public List<int> links = new List<int>();
    public List<int> episodes = new List<int>();
    public bool changed { get; private set; }
}