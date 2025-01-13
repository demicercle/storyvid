using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum Panels
    {
        Home,
        SelectEpisode,
        SelectVideo,
        PlayVideo
    }

    public List<CSVReader> csvs = new List<CSVReader>();

    public StoryPlayer storyPlayer;
    public int currentPanel;
    public GameObject[] panels;

    public string language = "fr";
    
    public UnityEngine.UI.Button playButton;
    public UnityEngine.UI.Button quitButton;

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

    public CSVReader GetEpisodeCSV(int episode)
    {
        return csvs.FirstOrDefault(csv => csv.textAsset.name.ToLower() == "episode" + episode);
    }

    public string GetVideoID(int video)
    {
        return "video" + (video < 10 ? "0" : "") + video;
    }
    
    public void PlayStory(int episode, string videoID = null)
    {
        var episodeCSV = GetEpisodeCSV(episode);
        if (videoID == null)
            videoID = GetVideoIDs(episode).FirstOrDefault();
        storyPlayer.lines.Clear();
        var content = episodeCSV.GetCellContent(videoID, "script_" + language);
        storyPlayer.lines.AddRange(content.Split('\n'));
        storyPlayer.PlayVideo(episodeCSV.GetCellContent(videoID, "video_file"));
        storyPlayer.isPlaying = true;
        SetCurrentPanel(Panels.PlayVideo);
    }

    public bool GetImageForVideo(out Texture texture, int episode, string videoID = null)
    {
        texture = null;
        var episodeCSV = GetEpisodeCSV(episode);
        if (episodeCSV == null)
            return false;
        if (string.IsNullOrEmpty(videoID))
            videoID = GetVideoIDs(episode).FirstOrDefault();
        if (string.IsNullOrEmpty(videoID))
            return false;
        texture = Resources.Load<Texture>("Images/" + episodeCSV.GetCellContent(videoID, "image_file"));
        return false;
    }

    public string[] GetVideoIDs(int episode)
    {
        var episodeCSV = GetEpisodeCSV(episode);
        if (episodeCSV == null)
            return new string[0];
        else
            return episodeCSV.rowIDs.ToArray();
    }

    private void Awake()
    {
        Localize.LoadLines();

        csvs.ForEach(csv => csv.ParseTextAsset());
        
        SetCurrentPanel(0);
        
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
