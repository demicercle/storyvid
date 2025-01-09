using System;
using UnityEngine;

public class EpisodeElement : MonoBehaviour
{
    public int episodeIndex;
    public TMPro.TMP_Text header;
    public UnityEngine.UI.Button playButton;

    private GameManager gameManager;
    
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        header.text = "Episode " + episodeIndex;
        playButton.onClick.AddListener(() =>
        {
            gameManager.SetCurrentPanel(GameManager.Panels.PlayVideo);
            gameManager.storyPlayer.isPlaying = true;
            gameManager.storyPlayer.inkStory.ChoosePathString("episode_" + episodeIndex );
        });
    }
}
