using System;
using UnityEngine;

public class EpisodeVideoElement : MonoBehaviour
{
    public UnityEngine.UI.RawImage imageComponent;
    public UnityEngine.UI.Button button;
    
    public string path;

    private GameManager gameManager;
    private Texture pathImage;
    
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        button.onClick.AddListener(() =>
        {
            gameManager.PlayPath(path);
        });
    }

    private void Update()
    {
        button.interactable = gameManager.storyPlayer.IsPathUnlocked(path);

        if (pathImage == null)
        {
            if (gameManager.GetImageForPath(path, out pathImage))
                imageComponent.texture = pathImage;
        }
    }
}
