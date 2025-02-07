using System;
using UnityEngine;

public class EpisodeVideoElement : MonoBehaviour
{
    public UnityEngine.UI.RawImage imageComponent;
    public UnityEngine.UI.Button button;

    public string videoID;
    public int episode;

    private GameManager gameManager => GameManager.instance;
    private Texture pathImage;

    public void UpdateImage()
    {
        if (gameManager.GetImageForVideo(out pathImage, episode, videoID))
            imageComponent.texture = pathImage;
    }
    
    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            gameManager.PlayVideo(episode, videoID);
        });
    }

    private void Update()
    {
        button.interactable = gameManager.storyPlayer.IsPathUnlocked(episode, videoID);
    }
}
