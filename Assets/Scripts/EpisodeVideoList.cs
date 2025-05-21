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
    private GameManager gameManager => GameManager.instance;

    private void Awake()
    {
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
        
        GameManager.onLanguageChanged += LanguageChanged;
    }

    private void OnDestroy()
    {
        GameManager.onLanguageChanged -= LanguageChanged;
    }

    private void Start()
    {
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

    private TMPro.TMP_FontAsset defaultFont;
    private void LanguageChanged()
    {
        if (defaultFont == null) defaultFont = header.font;
        header.font = GameManager.instance.GetLanguageTMPFontAsset(out var newFont) ? newFont : defaultFont;
        header.text = GameManager.instance.GetEpisodeHeader(episodeIndex);
    }
}
