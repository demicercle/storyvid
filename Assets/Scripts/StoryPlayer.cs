using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.Linq;

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

    public bool GetLastUnlockedVideo(int episode, out string videoID)
    {
        videoID = GameManager.instance.GetVideoIDs(episode).LastOrDefault(id => IsPathUnlocked(episode, id));
        return !string.IsNullOrEmpty(videoID);
    }
    
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

        isPlaying = true;
        videoPlayer.clip = Resources.Load<VideoClip>("Videos/" + file) as VideoClip;
        videoPlayer.time = 0;
        videoPlayer.Play();

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
        
        isPlaying = false;
    }

    public void StopMusic()
    {
        MusicPlayer.StopMusic();
    }

    private void Awake()
    {
        nextButton.gameObject.SetActive(false);
        nextButton.onClick.AddListener(() =>
        {
            lineIndex += 1;
            displayContent = lastContent = string.Empty;
        });
        
        prevButton.gameObject.SetActive(false);
        prevButton.onClick.AddListener(() =>
        {
            lineIndex = Mathf.Max(0, lineIndex - 1);
            displayContent = lastContent = string.Empty;
        });
        
        choiceButtons.ForEach( (btn) =>
        {
            btn.gameObject.SetActive(false);
            btn.userData = choiceButtons.IndexOf(btn);
            btn.onClick += (userData) =>
            {
                displayContent = lastContent = string.Empty;
                nextVideo = ((VideoLink)userData).videoTo;
                lineIndex = lines.Count;
            };
        });
    }

    IEnumerator Play()
    {
        Debug.Log(this + " Play");
        
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
        
        while (isPlaying)
        {
            fader.Fade0();
            
            while (fader.isFading)
                yield return null;
            
            while (lineIndex < lines.Count)
            {
                Debug.Log("lineIndex: " + lineIndex + " / " + lines.Count);
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
                }

                var firstLine = lineIndex == 0;
                var lastLine = lineIndex + 1 >= lines.Count;
                var hasLinks = links.Count > 1;
                
                nextButton.gameObject.SetActive(!lastLine || !hasLinks);
                prevButton.gameObject.SetActive(!firstLine);

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
                
                while (!string.IsNullOrWhiteSpace(lastContent))
                {
                    yield return null;
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

    private void LateUpdate()
    {
        uiText.enabled = !string.IsNullOrWhiteSpace(displayContent);
        uiText.text = displayContent;
    }
}
