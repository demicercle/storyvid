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
    
    private void Awake()
    {
        header.text = "Episode " + episodeIndex;
        playButton.onClick.AddListener(() =>
        {
            gameManager.PlayEpisode(episodeIndex);
        });
        continueButton.onClick.AddListener(() =>
        {
            gameManager.SetCurrentPanel(GameManager.Panels.SelectVideo);
        });
    }

    private void Update()
    {
        if (pathImage == null)
        {
            gameManager.GetImageForVideo(out pathImage, episodeIndex);
            imageContainer.texture = pathImage;
        }
    }
}
