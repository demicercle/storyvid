using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ink.UnityIntegration;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GameManager : MonoBehaviour
{
    public enum Panels
    {
        Home,
        SelectEpisode,
        SelectVideo,
        PlayVideo
    }

    public StoryPlayer storyPlayer;
    public int currentPanel;
    public GameObject[] panels;

    public string language = "fr";
    
    public UnityEngine.UI.Button playButton;
    public UnityEngine.UI.Button quitButton;

    public TextAsset jsonAsset;
    public Newtonsoft.Json.Linq.JObject jsonData;
    public List<VideoLink> allLinks;
    
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
    
    public void PlayStory(int episode, string videoID = null)
    {
        if (string.IsNullOrEmpty(videoID))
            videoID = GetVideoIDs(episode).FirstOrDefault();
        var content = GetVideoContent(episode, videoID);
        storyPlayer.lines.AddRange(content.Split('\n'));
        storyPlayer.links = GetLinks(episode, videoID);
        storyPlayer.episode = episode;
        storyPlayer.PlayVideo(GetVideoFile(episode, videoID));
        storyPlayer.isPlaying = true;
        storyPlayer.UnlockPath(episode, videoID);
        SetCurrentPanel(Panels.PlayVideo);
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

    private void ParseLinks()
    {
        allLinks.Clear();
        
        var linksData = jsonData["links"] as JArray;
        foreach (var data in linksData)
        {
            var link = new VideoLink();
            link.episode = int.Parse(data["episode"].ToString());
            link.videoFrom = data["video_from"].ToString();
            link.videoTo = data["video_to"].ToString();
            link.text = data["text_" + language].ToString();
            allLinks.Add(link);
        }
    }
    
    [ContextMenu("Parse JSON")]
    private void ParseJson()
    {
        Debug.Log("ParseJson:" + jsonAsset.text);
        jsonData = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(jsonAsset.text);
        ParseLinks();
        Debug.Log(GetLocalizedText("play"));
        Debug.Log(GetVideoContent(1, "video01"));
        Debug.Log(string.Join(",", GetVideoIDs(1)));
    }

    public string GetLocalizedText(string key)
    {
        return ((jsonData["interface"] as JObject)[key] as JObject)["text_" + language].ToString();
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
        return allLinks.Where(link => link.episode == episode && link.videoFrom == videoFrom).ToList();
    }

    private void Awake()
    {
        Localize.LoadLines();
        
        ParseJson();
        
        SetCurrentPanel(0);

        storyPlayer.storyComplete += () =>
        {
            if (!string.IsNullOrEmpty(storyPlayer.nextVideo))
            {
                PlayStory(storyPlayer.episode, storyPlayer.nextVideo);
                storyPlayer.nextVideo = string.Empty;
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
