using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.Linq;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StoryPlayer : MonoBehaviour
{
    public float textDelay;
    public VideoPlayer videoPlayer;

    public Fader fader;
    public TMPro.TMP_Text uiText;
    public UnityEngine.UI.Button nextButton;
    public UnityEngine.UI.Button prevButton;
    public List<CustomButton> choiceButtons;
    
    public bool isPlaying { get; private set; }

    public System.Action storyComplete;

    public List<string> lines = new List<string>();
    public List<VideoLink> links = new List<VideoLink>();
    
    public int episode;
    public string nextVideo;
    public string musicFile;
    
    //public GameManager gameManager;
    
    private string lastContent;
    private string displayContent;
    private int charIndex;
    private int lineIndex;
    private int choiceCount;
    private List<string> knots;
    private bool isFadingMusic;

    public bool autoPlay => autoPlayToggle.isOn;
    
    public bool canDoubleClick;
    public Toggle autoPlayToggle;
    
    private float lastClickTime;
    private bool doubleClicked;

    public void PlayVideo(string file)
    {
        if (videoPlayer.clip != null)
        {
            videoPlayer.Stop();
            Resources.UnloadAsset(videoPlayer.clip);
        }

        isPlaying = true;
        videoPlayer.clip = Resources.Load<VideoClip>("Videos/" + file) as VideoClip;
        videoPlayer.time = 0;
        videoPlayer.Play();

        if (videoPlayer.clip == null)
        {
            Debug.LogError("Unable to find video: " + file);
        }

        StopCoroutine("Play");
        StartCoroutine("Play");
    }

    public void StopVideo()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
            Resources.UnloadAsset(videoPlayer.clip);
            videoPlayer.clip = null;
        }
        
        Debug.Log("Stop Video");
        isPlaying = false;
    }

    public void StopMusic()
    {
        MusicPlayer.StopMusic();
    }

    public bool Skip()
    {
        return Application.isEditor && Input.GetKey(KeyCode.Backspace);
    }

    public void Next()
    {
        lineIndex += 1;
        displayContent = lastContent = string.Empty;
    }

    public void Prev()
    {
        lineIndex = Mathf.Max(0, lineIndex - 1);
        displayContent = lastContent = string.Empty;
    }
    
    private void Awake()
    {
        autoPlayToggle.isOn = PlayerPrefs.GetInt("autoPlay", 0) == 1;
        autoPlayToggle.onValueChanged.AddListener((isOn) =>
        {
            PlayerPrefs.SetInt("autoPlay", isOn ? 1 : 0);
        });
        
        nextButton.gameObject.SetActive(false);
        nextButton.onClick.AddListener(Next);
        
        prevButton.gameObject.SetActive(false);
        prevButton.onClick.AddListener(Prev);
        
        choiceButtons.ForEach( (btn) =>
        {
            btn.gameObject.SetActive(false);
            btn.userData = choiceButtons.IndexOf(btn);
            btn.onClick += (userData) =>
            {
                var link = (VideoLink)userData;
                displayContent = lastContent = string.Empty;
                nextVideo = link.videoTo;
                lineIndex = lines.Count;
                GameManager.instance.SetVisitedLink(link.id, true);
            };
        });
    }

    IEnumerator Play()
    {
        Debug.Log("PlayVideo: " + videoPlayer.clip + " nextVideo: " + nextVideo);
        
        lineIndex = 0;
        charIndex = 0;
        displayContent = lastContent = string.Empty;
        nextButton.gameObject.SetActive(false);
        prevButton.gameObject.SetActive(false);
        choiceButtons.ForEach(btn => btn.gameObject.SetActive(false));

        if (!string.IsNullOrEmpty(musicFile))
        {
            MusicPlayer.PlayMusic(musicFile);
        }
        else
        {
            MusicPlayer.StopMusic();
        }
        
        while (videoPlayer.clip != null && isPlaying)
        {
            fader.Fade0();
            
            while (fader.isFading)
                yield return null;
            
            while (lineIndex < lines.Count)
            {
                Debug.Log("line: " + (lineIndex+1) + " / " + lines.Count);
                lastContent = lines[lineIndex];
                
                charIndex = 0;
                    
                while (charIndex < lastContent.Length)
                {
                    var ch = lastContent[charIndex];
                    if (ch == '@')
                    {
                        var delayStr = "";
                        do
                        {
                            lastContent = lastContent.Remove(charIndex, 1);
                            delayStr += lastContent[charIndex];
                        } while (charIndex < lastContent.Length && lastContent[charIndex] != ' ');
                        var delay = 0;
                        if(!int.TryParse(delayStr, out delay))
                            Debug.LogError("Unable to parse delay " + delayStr + " in content: " + lastContent);
                        else
                        {
                            Debug.Log("Wait: " + delay);
                            yield return new WaitForSeconds((float)delay / 100f);
                        }
                    }
                    else
                    {
                        displayContent = lastContent.Substring(0, charIndex);
                        charIndex++;
                        yield return Input.GetMouseButton(0) ? null : new WaitForSeconds(textDelay);
                    }

                    if (Skip())
                    {
                        displayContent = lastContent;
                        charIndex = lastContent.Length;
                    }
                }

                var firstLine = lineIndex == 0;
                var lastLine = lineIndex + 1 >= lines.Count;
                var hasLinks = links.Count > 1;

                if (!firstLine)
                {
                    prevButton.gameObject.SetActive(true);
                }

                if (lastLine && hasLinks)
                {
                    if (links.Count > 1)
                    {
                        for (int i = 0; i < links.Count; i++)
                        {
                            var btn = choiceButtons[i];
                            btn.gameObject.SetActive(true);
                            btn.SetText(links[i].text);
                            btn.userData = links[i];
                        }
                    }
                }
                else
                {
                    nextButton.gameObject.SetActive(true);
                }

                if (autoPlay && links.Count <= 1)
                {
                    nextButton.gameObject.SetActive(false);
                    prevButton.gameObject.SetActive(false);
                    yield return new WaitForSeconds(1f);
                    Next();
                }
                else
                {
                    while (!string.IsNullOrWhiteSpace(lastContent))
                    {
                        if (doubleClicked && links.Count <= 1)
                        {
                            Next();
                        }
                    
                        yield return null;
                    }
                }
                    
                nextButton.gameObject.SetActive(false);
                prevButton.gameObject.SetActive(false);
                choiceButtons.ForEach(btn => btn.gameObject.SetActive(false));
            }

            fader.Fade1();
            while (fader.isFading)
                yield return null;

            StopVideo();
        }
        
        storyComplete?.Invoke();
    }

    private void Update()
    {
        if (canDoubleClick)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (Time.time - lastClickTime < .5f)
                {
                    doubleClicked = true;
                    lastClickTime = 0;
                }
                else
                {
                    lastClickTime = Time.time;
                }
            }
        }
    }

    private void LateUpdate()
    {
        uiText.enabled = !string.IsNullOrWhiteSpace(displayContent);
        uiText.text = displayContent;
        doubleClicked = false;
    }
}
