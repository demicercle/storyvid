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
    
    private List<EpisodeVideoElement> videoElements;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        videoElements = new List<EpisodeVideoElement>();
        videoElements.AddRange(GetComponentsInChildren<EpisodeVideoElement>());
        
        foreach (var episodeVideoElement in videoElements)
        {
            episodeVideoElement.gameObject.SetActive(false);
        }
        
        episodeButton.onClick.AddListener(() =>
        {
            gameManager.PlayEpisode(episodeIndex);
        });
    }

    private void Start()
    {
        header.text = "Episode " + episodeIndex.ToString();

        var lastVideoElement = 0;
        foreach (string videoID in gameManager.GetVideoIDs(episodeIndex))
        {
          //  Debug.Log(lastVideoElement + " " + videoID);
            videoElements[lastVideoElement].gameObject.SetActive(true);
            videoElements[lastVideoElement].episode = episodeIndex;
            videoElements[lastVideoElement].videoID = videoID;
            videoElements[lastVideoElement].UpdateImage();
            lastVideoElement++;
        }
    }
}
