using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.Video;

public class StoryPlayer : MonoBehaviour
{
    public TextAsset storyFile;
    public float textDelay;
    public VideoPlayer videoPlayer;

    public Fader fader;
    public TMPro.TMP_Text uiText;
    public UnityEngine.UI.Button nextButton;
    public List<CustomButton> choiceButtons;

    public bool isPlaying;
    public Story inkStory { get; private set; }

    public System.Action storyComplete;
    
    private string lastContent;
    private string displayContent;
    private int charIndex;
    private int choiceCount;

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
                inkStory.ChooseChoiceIndex((int)userData);
            };
        });
        
        inkStory = new Story(storyFile.text);
        
        inkStory.BindExternalFunction ("playVideo", (string file) =>
        {
            Debug.Log("playVideo() " + file);
            
            if (videoPlayer.clip != null)
            {
                videoPlayer.Stop();
                Resources.UnloadAsset(videoPlayer.clip);
            }
            
            videoPlayer.clip = Resources.Load<VideoClip>("Videos/" + file) as VideoClip;
            videoPlayer.Play();
        });
    }

    IEnumerator Start()
    {
        while (true)
        {
            while (!isPlaying)
                yield return null;
            
            fader.Fade0();
            
            while (inkStory.canContinue)
            {
                lastContent = inkStory.Continue();

                while (fader.isFading)
                    yield return null;
                
                charIndex = 0;
                while (charIndex < lastContent.Length)
                {
                    displayContent = lastContent.Substring(0, charIndex);
                    charIndex++;
                    yield return Input.GetMouseButton(0) ? null : new WaitForSeconds(textDelay);
                }
                
                choiceCount = inkStory.currentChoices.Count;
                for (int i = 0; i < choiceButtons.Count; i++)
                {
                    if (i < choiceCount)
                    {
                        choiceButtons[i].gameObject.SetActive(true);
                        choiceButtons[i].SetText(inkStory.currentChoices[i].text);
                    }
                    else
                    {
                        choiceButtons[i].gameObject.SetActive(false);
                    }
                }

                nextButton.gameObject.SetActive(choiceCount == 0);
                
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