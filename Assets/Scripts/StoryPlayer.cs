using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.Video;
using System.Linq;

public class StoryPlayer : MonoBehaviour
{
    public float textDelay;
    public VideoPlayer videoPlayer;

    public Fader fader;
    public TMPro.TMP_Text uiText;
    public UnityEngine.UI.Button nextButton;
    public List<CustomButton> choiceButtons;

    public bool isPlaying;

    public System.Action storyComplete;

    public List<string> lines = new List<string>();
    public List<VideoLink> links = new List<VideoLink>();
    
    public bool IsPathUnlocked(int episode, string videoID)
    {
        return PlayerPrefs.GetInt("episode" + episode + "." + videoID) != 0;
    }

    public void UnlockPath(int episode, string videoID)
    {
        PlayerPrefs.SetInt("episode" + episode + "." + videoID, 1);
    }

    public void PlayVideo(string file)
    {
        if (videoPlayer.clip != null)
        {
            videoPlayer.Stop();
            Resources.UnloadAsset(videoPlayer.clip);
        }
            
        videoPlayer.clip = Resources.Load<VideoClip>("Videos/" + file) as VideoClip;
        videoPlayer.Play();
    }

    public GameManager gameManager;
    
    private string lastContent;
    private string displayContent;
    private int charIndex;
    private int choiceCount;
    private List<string> knots;
    private int selectChoice = -1;


    private void Awake()
    {
        nextButton.gameObject.SetActive(false);
        nextButton.onClick.AddListener(() =>
        {
            displayContent = lastContent = string.Empty;
        });
        
        choiceButtons.ForEach( (btn) =>
        {
            btn.gameObject.SetActive(false);
            btn.userData = choiceButtons.IndexOf(btn);
            btn.onClick += (userData) =>
            {
                displayContent = lastContent = string.Empty;
                choiceButtons.ForEach(btn => btn.gameObject.SetActive(false));
            };
        });
    }

    IEnumerator Start()
    {
        while (true)
        {
            while (!isPlaying)
                yield return null;
            
            fader.Fade0();
            while (fader.isFading)
                yield return null;
            
            while (lines.Count > 0)
            {
                lastContent = lines[0];
                lines.RemoveAt(0);

                charIndex = 0;
                while (charIndex < lastContent.Length)
                {
                    displayContent = lastContent.Substring(0, charIndex);
                    charIndex++;
                    yield return Input.GetMouseButton(0) ? null : new WaitForSeconds(textDelay);
                }

                nextButton.gameObject.SetActive(true);
                
                if (lines.Count == 0)
                {
                    for (int i = 0; i < links.Count; i++)
                    {
                        nextButton.gameObject.SetActive(false);
                        
                        var btn = choiceButtons[i];
                        btn.gameObject.SetActive(true);
                        btn.SetText(links[i].text);
                        btn.userData = links[i];
                    }
                }
                
                while (!string.IsNullOrWhiteSpace(lastContent))
                    yield return null;
                
                nextButton.gameObject.SetActive(false);
                choiceButtons.ForEach(btn => btn.gameObject.SetActive(false));
            }

            fader.Fade1();
            while (fader.isFading)
                yield return null;

            isPlaying = false;
            videoPlayer.Stop();
            storyComplete?.Invoke();
            
            yield return null;
        }
    }

    private void LateUpdate()
    {
        uiText.enabled = !string.IsNullOrWhiteSpace(displayContent);
        uiText.text = displayContent;
    }
}
