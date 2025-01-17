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
            gameManager.PlayPath("episode_" + episodeIndex);
        });
    }

    private void Start()
    {
        header.text = "Episode " + episodeIndex.ToString();
        
        foreach (var episodeVideoElement in videoElements)
        {
            episodeVideoElement.gameObject.SetActive(false);
        }

        var knots = gameManager.storyPlayer.GetKnots();
        var lastVideoElement = 0;
        foreach (string knot in knots)
        {
            if (knot.StartsWith("episode_" + episodeIndex.ToString()) && knot.Split(".").Length > 1)
            {
                videoElements[lastVideoElement].gameObject.SetActive(true);
                videoElements[lastVideoElement].path = knot;
            }
            
            lastVideoElement++;
        }
    }
}
