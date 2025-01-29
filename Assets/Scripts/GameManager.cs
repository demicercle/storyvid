using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ink.UnityIntegration;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    static public GameManager instance { get; private set; }
    
    public enum Panels
    {
        Home,
        SelectEpisode,
        SelectVideo,
        PlayVideo,
        Options
    }

    public StoryPlayer storyPlayer;
    public int currentPanel;
    public GameObject[] panels;

    public string language { get; private set; }

   static public System.Action onLanguageChanged;
    public void SetLanguage(string key)
    {
        language = key;
        onLanguageChanged?.Invoke();
    }
    
    public UnityEngine.UI.Button playButton;
    public UnityEngine.UI.Button quitButton;
    
    public TextAsset jsonAsset;
    public Newtonsoft.Json.Linq.JObject jsonData;
    public List<VideoLink> videoLinks;
    public List<MusicLink> musicLinks;
    
    private void OnDrawGizmosSelected()
    {
        UpdatePanels();
    }

    public void SetCurrentPanel(Panels panel)
    {
        currentPanel = (int)panel;
    }

    public void SetCurrentPanel(int panel)
    {
        currentPanel = panel;
    }

    public string GetVideoID(int video)
    {
        return "video" + (video < 10 ? "0" : "") + video;
    }
    
    public void PlayEpisode(int episode)
    {
        PlayVideo(episode, GetVideoIDs(episode).FirstOrDefault());
    }
    
    public void PlayVideo(int episode, string videoID)
    {
        var content = GetVideoContent(episode, videoID);
        storyPlayer.lines.Clear();
        storyPlayer.lines.AddRange(content.Split('\n'));
        storyPlayer.links = GetLinks(episode, videoID);
        storyPlayer.episode = episode;
        storyPlayer.nextVideo = GetNextVideoID(episode, videoID);
        storyPlayer.musicFile = GetMusicFile(episode, videoID);
        storyPlayer.PlayVideo(GetVideoFile(episode, videoID));
        storyPlayer.UnlockPath(episode, videoID);
        SetCurrentPanel(Panels.PlayVideo);
    }

    public void StopVideo()
    {
        storyPlayer.StopVideo();
        storyPlayer.StopMusic();
        SetCurrentPanel((int)Panels.SelectVideo);
    }

    public string GetMusicFile(int episode, string videoID)
    {
        return musicLinks.FirstOrDefault(m => m.episode == episode && m.videoFrom == videoID).musicFile;
    }

    public bool GetImageForVideo(out Texture texture, int episode, string videoID = null)
    {
        texture = null;
        if (string.IsNullOrEmpty(videoID))
            videoID = GetVideoIDs(episode).FirstOrDefault();
        if (!string.IsNullOrEmpty(videoID))
            texture = Resources.Load<Texture>("Images/" + GetVideoImageFile(episode, videoID));
        return texture != null;
    }

    public string[] GetVideoIDs(int episode)
    {
        var keys = new List<string>();

        if (jsonData.ContainsKey("episode" + episode))
        {
            foreach (JProperty jProp in (jsonData["episode" + episode] as JObject).Properties())
            {
                keys.Add(jProp.Name.ToString());
            }
        }

        return keys.ToArray();
    }

    public string GetNextVideoID(int episode, string videoID)
    {
        var videoIDs = GetVideoIDs(episode).ToList();
        var videoIndex = videoIDs.IndexOf(videoID);
        videoIndex += 1;
        if (videoIndex < videoIDs.Count)
            return videoIDs[videoIndex];
        else
            return string.Empty;
    }

    private void ParseLinks()
    {
        videoLinks.Clear();
        
        var linksData = jsonData["links"] as JArray;
        foreach (var data in linksData)
        {
            var link = new VideoLink();
            link.episode = int.Parse(data["episode"].ToString());
            link.videoFrom = data["video_from"].ToString();
            link.videoTo = data["video_to"].ToString();
            link.text = data["text_" + language].ToString();
            videoLinks.Add(link);
        }
    }

    private void ParseMusics()
    {
        musicLinks.Clear();

        var musicsData = jsonData["musics"] as JArray;
        foreach (var data in musicsData)
        {
            var music = new MusicLink();
            music.episode = int.Parse(data["episode"].ToString());
            music.videoFrom = data["video"].ToString();
            music.musicFile = data["music"].ToString();
            musicLinks.Add(music);
        }
    }
    
    [ContextMenu("Parse JSON")]
    private void ParseJson()
    {
        Debug.Log("ParseJson:" + jsonAsset.text);
        jsonData = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(jsonAsset.text);
        ParseLinks();
        ParseMusics();
    }

    public string GetLocalizedText(string key)
    {
        var jo = (jsonData["interface"] as JObject);
        if (!jo.ContainsKey(key))
        {
            Debug.LogError("localize key not found: " + key);
        }
        jo = jo[key] as JObject;
        if (!jo.ContainsKey("text_" + language))
        {
            Debug.LogError("localized language " + language + " not found in key: " + key);
        }
        return jo["text_" + language].ToString();
    }

    public JObject GetVideoData(int episode, string videoID)
    {
        var episodeData = jsonData["episode" + episode] as JObject;
        if (episodeData != null)
            return (episodeData[videoID] as JObject);
        else
            return null;
    }

    public string GetVideoImageFile(int episode, string videoID)
    {
        var videoData = GetVideoData(episode, videoID);
        if (videoData != null)
            return videoData["image_file"].ToString();
        else
            return string.Empty;
    }

    public string GetVideoFile(int episode, string videoID)
    {
        var videoData = GetVideoData(episode, videoID);
        if (videoData != null)
            return videoData["video_file"].ToString();
        else
            return string.Empty;
    }

    public string GetVideoContent(int episode, string videoID)
    {
        var videoData = GetVideoData(episode, videoID);
        if (videoData != null)
            return videoData["script_" + language].ToString();
        else
            return string.Empty;
    }

    public List<VideoLink> GetLinks(int episode, string videoFrom)
    {
        return videoLinks.Where(link => link.episode == episode && link.videoFrom == videoFrom).ToList();
    }

    private void Awake()
    {
        instance = this;
        language = "fr";
        
        ParseJson();
        
        SetCurrentPanel(0);

        storyPlayer.storyComplete += () =>
        {
            if (!string.IsNullOrEmpty(storyPlayer.nextVideo))
            {
                PlayVideo(storyPlayer.episode, storyPlayer.nextVideo);
            }
            else
            {
                SetCurrentPanel(Panels.SelectVideo);
            }
        };
        
        playButton.onClick.AddListener(() =>
        {
            SetCurrentPanel((int)Panels.SelectEpisode);
        });
        
        quitButton.onClick.AddListener(() =>
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        });
    }

    private void Update()
    {
        UpdatePanels();
    }

    private void UpdatePanels()
    {
        currentPanel = Mathf.Clamp(currentPanel, 0, panels.Length - 1);
        for (int i = 0; i < panels.Length; ++i)
        {
            panels[i].SetActive(i == currentPanel);
        }
    }
}
