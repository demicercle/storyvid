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
    private bool doUpdate;
    
    private void Awake()
    {
        header.text = GameManager.instance.GetEpisodeHeader(episodeIndex);

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
        doUpdate = true;
    }

    private void LateUpdate()
    {
        if (!doUpdate) return;
        
        doUpdate = false;
        gameManager.GetLastUnlockedVideo(episodeIndex, out lastVideoID);
        
        lockedToggle.isOn = !gameManager.savedGame.IsEpisodeUnlocked(episodeIndex);
        
        if (pathImage == null)
        {
            gameManager.GetImageForVideo(out pathImage, episodeIndex);
            imageContainer.texture = pathImage;
        }

        unlockedOverlay.SetActive(lockedToggle.isOn);
        playButton.interactable = !lockedToggle.isOn;
        continueButton.interactable = !lockedToggle.isOn && !string.IsNullOrEmpty(lastVideoID);
        continueButton.gameObject.SetActive(continueButton.interactable);
        playButton.gameObject.SetActive(!continueButton.interactable);
        
        imageContainer.texture = Resources.Load<Texture2D>("Textures/portrait" + episodeIndex);
    }
}
