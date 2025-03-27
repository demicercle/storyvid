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
    static public float[] speeds = new float[] { 0.30f, 0.15f, 0.015f };

    static public int selectedSpeed
    {
        get => PlayerPrefs.GetInt("textSpeed", 0);
        set
        {
            PlayerPrefs.SetInt("textSpeed", value);
            PlayerPrefs.Save();
        }
    }

    static public float textDelay => speeds[selectedSpeed];

    static public float autoPlayDuration => 2f;
    
    public VideoPlayer videoPlayer;

    public Fader fader;
    public TMPro.TMP_Text uiText;
    public Button nextButton;
    public Button prevButton;
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
        videoPlayer.isLooping = true;
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

    public void NextLineAndClear()
    {
        lineIndex += 1;
        Debug.Log("next line: " + lineIndex);
        displayContent = lastContent = string.Empty;
    }

    public void PrevLineAndClear()
    {
        lineIndex = Mathf.Max(0, lineIndex - 1);
        Debug.Log("prev line: " + lineIndex);
        displayContent = lastContent = string.Empty;
    }
    
    private void Awake()
    {
        autoPlayToggle.isOn = PlayerPrefs.GetInt("autoPlay", 0) == 1;
        autoPlayToggle.onValueChanged.AddListener((isOn) =>
        {
            PlayerPrefs.SetInt("autoPlay", isOn ? 1 : 0);
            PlayerPrefs.Save();
        });
        
        nextButton.gameObject.SetActive(false);
        nextButton.onClick.AddListener(NextLineAndClear);
        
        prevButton.gameObject.SetActive(false);
        prevButton.onClick.AddListener(PrevLineAndClear);
        
        choiceButtons.ForEach( (btn) =>
        {
            btn.gameObject.SetActive(false);
            btn.userData = choiceButtons.IndexOf(btn);
            btn.onClick += (userData) =>
            {
                ChooseLink((VideoLink)userData);
            };
        });
    }

    private void ChooseLink(VideoLink link)
    {
        Debug.Log("ChooseLink: " + link.videoTo);
        displayContent = lastContent = string.Empty;
        nextVideo = link.videoTo;
        lineIndex = lines.Count;
        GameManager.instance.SetVisitedLink(link.id, true);
        if (link.EpisodeComplete())
            GameManager.instance.SetEpisodeCompleted(link.episode, true);
    }

    private bool waitVideoEnd;
    private const string CINEMATIC = "cinematique";
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
                var lastLine = lineIndex + 1 >= lines.Count;
                Debug.Log("line #" + lineIndex + " / " + lines.Count + " last:" + lastLine + " links:" + links.Count);
                
                yield return null;
                
                lastContent = lines[lineIndex];
                charIndex = 0;
                
                while (charIndex < lastContent.Length)
                {
                    var ch = lastContent[charIndex];
                    if (ch == '@')
                    {
                        charIndex += 1;
                        
                        if (lastContent.Substring(charIndex, CINEMATIC.Length) == CINEMATIC)
                        {
                            lastContent = string.Empty;
                            waitVideoEnd = true;
                        }
                        
                        else if (int.TryParse(lastContent.Substring(charIndex, 1), out var _))
                        {
                            var delayStr = "";
                            do
                            {
                                lastContent = lastContent.Remove(charIndex, 1);
                                delayStr += lastContent[charIndex];
                            } while (charIndex < lastContent.Length && lastContent[charIndex] != ' ');
                            
                            if (int.TryParse(delayStr, out var delay))
                            {
                                Debug.Log("Wait: " + delay);
                                yield return new WaitForSeconds((float)delay / 100f);
                            }
                            else
                            {
                                Debug.LogError("Unable to parse delay " + delayStr + " in content: " + lastContent);
                            }
                        }
                        
                        else
                        {
                            Debug.LogError("Unknown command @" + lastContent.Substring(charIndex));
                        }
                    }
                    else
                    {
                        displayContent = lastContent.Substring(0, charIndex + 1);
                        charIndex++;
                        yield return Input.GetMouseButton(0) ? null : new WaitForSeconds(textDelay);
                    }

                    if (Skip())
                    {
                        displayContent = lastContent;
                        charIndex = lastContent.Length;
                    }
                }

                if (waitVideoEnd)
                {
                    waitVideoEnd = false;
                    Debug.Log("Wait video end");
                    videoPlayer.isLooping = false;
                    while (videoPlayer.isPlaying)
                    {
                        if (Skip())
                            break;
                        
                        yield return null;
                    }
                    videoPlayer.Pause();
                }

                if (autoPlay)
                {
                    Debug.Log("autoplay next video");
                    yield return new WaitForSeconds(autoPlayDuration);
                    NextLineAndClear();
                }
                else
                {
                    if (!lastLine || links.Count <= 1)
                    {
                        Debug.Log("show next/prev buttons");
                        nextButton.gameObject.SetActive(true);
                        prevButton.gameObject.SetActive(lineIndex >= 1);
                        while (!string.IsNullOrEmpty(lastContent)) // will be skipped if "wait video end"
                        {
                            if (doubleClicked)
                                NextLineAndClear();
                            
                            yield return null;
                        }
                    }
                }
                
                if(lastLine)
                {
                    if (links.Count > 1)
                    {   
                        Debug.Log("display choices: " + links.Count);
                        for (int i = 0; i < links.Count; i++)
                        {
                            var btn = choiceButtons[i];
                            btn.gameObject.SetActive(true);
                            btn.SetText(links[i].text);
                            btn.userData = links[i];
                        }
                        
                        while (!string.IsNullOrEmpty(lastContent))
                        {
                            yield return null;
                        }
                    }
                    else
                    {
                        ChooseLink(links.First()); 
                    }
                }
                    
                nextButton.gameObject.SetActive(false);
                prevButton.gameObject.SetActive(false);
                choiceButtons.ForEach(btn => btn.gameObject.SetActive(false));
                
                yield return null;
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
