using System;
using UnityEngine;
using UnityEngine.UI;

public class EpisodeElement : MonoBehaviour
{
    public string episodeID => "episode_" + episodeIndex;
    
    public int episodeIndex;
    public TMPro.TMP_Text header;
    public Button playButton;
    public Button continueButton;
    public RawImage imageContainer;
    
    private GameManager gameManager => GameManager.instance;
    private Texture pathImage;
    private string lastVideoID;
    
    private void Awake()
    {
        header.text = "Episode " + episodeIndex;

        if (playButton != null)
        {
            playButton.onClick.AddListener(() =>
            {
                gameManager.PlayEpisode(episodeIndex);
            });
        }
        
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(() =>
            {
                gameManager.PlayVideo(episodeIndex, lastVideoID);
                //gameManager.SetCurrentPanel(GameManager.Panels.SelectVideo);
            });
        }
    }

    private void OnEnable()
    {
        gameManager.storyPlayer.GetLastUnlockedVideo(episodeIndex, out lastVideoID);
        
        if (pathImage == null)
        {
            gameManager.GetImageForVideo(out pathImage, episodeIndex);
            imageContainer.texture = pathImage;
        }
        
        if (continueButton != null)
        {
            continueButton.interactable = !string.IsNullOrEmpty(lastVideoID);
        }
    }
}
