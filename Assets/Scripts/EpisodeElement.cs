using System;
using UnityEngine;
using UnityEngine.UI;

public class EpisodeElement : MonoBehaviour
{
    public string episodePath => "episode_" + episodeIndex;
    
    public int episodeIndex;
    public TMPro.TMP_Text header;
    public Button playButton;
    public Button continueButton;
    public RawImage imageContainer;
    
    private GameManager gameManager;
    private Texture pathImage;
    
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        header.text = "Episode " + episodeIndex;
        playButton.onClick.AddListener(() =>
        {
            gameManager.PlayPath(episodePath);
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
            gameManager.GetImageForPath(episodePath, out pathImage);
            imageContainer.texture = pathImage;
        }
    }
}
