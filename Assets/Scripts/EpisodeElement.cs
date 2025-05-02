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
    public Toggle lockedToggle;
    public RawImage imageContainer;
    public GameObject unlockedOverlay;
    
    private GameManager gameManager => GameManager.instance;
    private Texture pathImage;
    private string lastVideoID;
    
    private void Awake()
    {
        header.text = "Episode " + episodeIndex;

        playButton.onClick.AddListener(() =>
        {
            gameManager.PlayEpisode(episodeIndex);
        });
        
        continueButton.onClick.AddListener(() =>
        {
            gameManager.PlayVideo(episodeIndex, lastVideoID);
        });
    }

    private void OnEnable()
    {
        gameManager.GetLastUnlockedVideo(episodeIndex, out lastVideoID);
        
        if (pathImage == null)
        {
            gameManager.GetImageForVideo(out pathImage, episodeIndex);
            imageContainer.texture = pathImage;
        }

        unlockedOverlay.SetActive(lockedToggle.isOn);
        playButton.interactable = !unlockedOverlay.activeSelf;
        continueButton.interactable = !unlockedOverlay.activeSelf && !string.IsNullOrEmpty(lastVideoID);
        continueButton.gameObject.SetActive(continueButton.interactable);
        playButton.gameObject.SetActive(!continueButton.interactable);
        
        imageContainer.texture = Resources.Load<Texture2D>("Textures/portrait" + episodeIndex);
    }

    private void Update()
    {
        lockedToggle.isOn = !gameManager.IsEpisodeUnlocked(episodeIndex);
    }
}
