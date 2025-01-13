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

    public void PlayPath(string path)
    {
        storyPlayer.inkStory.ChoosePathString(path);
        storyPlayer.isPlaying = true;
        storyPlayer.storyComplete += () =>
        {
            storyPlayer.storyComplete = null;
            SetCurrentPanel(Panels.SelectVideo);
        };
        
        SetCurrentPanel(Panels.PlayVideo);
    }

    public bool GetImageForPath(string path, out Texture texture)
    {
        var tags = storyPlayer.inkStory.TagsForContentAtPath(path);
        if (tags != null)
        {
            foreach (string tag in tags)
            {
                if (tag.StartsWith("image:"))
                {
                    var assetPath = "Images/" + tag.Substring("image:".Length).Replace('"', ' ').Trim();
                    texture = Resources.Load<Texture>(assetPath);
                    if (texture != null)
                        return true;
                    else
                    {
                        Debug.LogWarning("Cannot find image: " + assetPath);
                    }
                }
            }
        }

        texture = null;
        return false;
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
