using System;
using UnityEngine;

public class EpisodeVideoElement : MonoBehaviour
{
    public UnityEngine.UI.Image imageComponent;
    public UnityEngine.UI.Button button;
    
    public string path;

    private GameManager gameManager;
    
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        button.onClick.AddListener(() =>
        {
            gameManager.SetCurrentPanel(GameManager.Panels.PlayVideo);
            gameManager.storyPlayer.isPlaying = true;
            gameManager.storyPlayer.inkStory.ChoosePathString(path);
        });
    }
}
