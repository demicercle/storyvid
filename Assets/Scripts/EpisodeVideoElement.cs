using System;
using UnityEngine;

public class EpisodeVideoElement : MonoBehaviour
{
    public UnityEngine.UI.RawImage imageComponent;
    public UnityEngine.UI.Button button;

    public string videoID;
    public int episode;

    private GameManager gameManager;
    private Texture pathImage;
    
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        button.onClick.AddListener(() =>
        {
            gameManager.PlayStory(episode, videoID);
        });
    }

    private void Update()
    {
        button.interactable = gameManager.storyPlayer.IsPathUnlocked(episode, videoID);

        if (pathImage == null)
        {
            if (gameManager.GetImageForVideo(out pathImage, episode, videoID))
                imageComponent.texture = pathImage;
        }
    }
}
