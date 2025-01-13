using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EpisodeVideoList : MonoBehaviour
{
    public int episodeIndex;
    public TMPro.TMP_Text header;
    public Button episodeButton;
    public List<EpisodeVideoElement> videoElements;
    
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        episodeButton.onClick.AddListener(() =>
        {
            gameManager.PlayStory(episodeIndex);
        });
    }

    private void Start()
    {
        header.text = "Episode " + episodeIndex.ToString();
        
        foreach (var episodeVideoElement in videoElements)
        {
            episodeVideoElement.gameObject.SetActive(false);
        }

        var lastVideoElement = 0;
        foreach (string videoID in gameManager.GetVideoIDs(episodeIndex))
        {
            videoElements[lastVideoElement].gameObject.SetActive(true);
            videoElements[lastVideoElement].episode = episodeIndex;
            videoElements[lastVideoElement].videoID = videoID;
            videoElements[lastVideoElement].UpdateImage();
            lastVideoElement++;
        }
    }
}
