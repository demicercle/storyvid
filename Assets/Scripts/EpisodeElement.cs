using System;
using UnityEngine;

public class EpisodeElement : MonoBehaviour
{
    public int episodeIndex;
    public TMPro.TMP_Text header;
    public UnityEngine.UI.Button playButton;
    public UnityEngine.UI.Button continueButton;

    private GameManager gameManager;
    
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        header.text = "Episode " + episodeIndex;
        playButton.onClick.AddListener(() =>
        {
            gameManager.PlayPath("episode_" + episodeIndex);
        });
        continueButton.onClick.AddListener(() =>
        {
            gameManager.SetCurrentPanel(GameManager.Panels.SelectVideo);
        });
    }
}
