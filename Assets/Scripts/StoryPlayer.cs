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
    static public bool autoPlay;
    static public float[] speeds = new float[] { 0.10f, 0.05f, 0.01f };

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

    public float autoPlayDuration = 2f;
    
    public VideoPlayer videoPlayer;

    public Fader fader;
    public TMPro.TMP_Text uiText;
    public Graphic textBackground;
    public Button nextButton;
    public Button prevButton;
    public List<CustomButton> clickableBackgroundButtons = new List<CustomButton>();
    public List<CustomButton> choiceButtons;
    public PanelButton backButton;
    
    public bool isPlaying { get; private set; }

    public System.Action storyComplete;

    public List<string> lines = new List<string>();
    public List<VideoLink> links = new List<VideoLink>();
    public RawImage rawImage;
    
    public int episode;
    public string videoFile;
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
    private bool mouseOverBackground;
    
    public bool canDoubleClick;
    
    private float lastClickTime;
    private bool doubleClicked;

    public void PlayVideo(string file)
    {
        StopVideo();

        videoFile = file;
        
        var asset = Resources.Load("Videos/" + file);
        if (asset is VideoClip clip)
        {
            rawImage.enabled = false;
            videoPlayer.enabled = true;
            videoPlayer.clip = clip;
            videoPlayer.time = 0;
            videoPlayer.isLooping = true;
            videoPlayer.Play();
            Debug.Log(this + " play video " + clip);
        }
        else if (asset is Texture2D tex)
        {
            rawImage.texture = tex;
            rawImage.enabled = true;
            videoPlayer.enabled = false;
            Debug.Log(this + " show tex " + tex);
        }
        else
        {
            Debug.LogError(this + " unknown file type: " + asset + " at path " + file);
            return;
        }
        
        isPlaying = true;

        StopCoroutine("Play");
        StartCoroutine("Play");
    }

    public void StopVideo()
    {
        if (isPlaying)
        {
            if (videoPlayer.clip != null)
            {
                videoPlayer.Stop();
                Resources.UnloadAsset(videoPlayer.clip);
                videoPlayer.clip = null;
            }
        
            StopCoroutine("Play");
            isPlaying = false;
            Debug.Log("Stop Video");
        }
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
        choiceButtons.ForEach(btn => btn.gameObject.SetActive(false));
        lineIndex -= 1;
        if (lineIndex >= 0)
        {
            Debug.Log("prev line: " + lineIndex);
            displayContent = lastContent = string.Empty;
        }
        /*else
        {
            var previousLinks = GameManager.instance.GetPreviousLinks(videoFile);
            foreach (var link in previousLinks)
            {
                Debug.Log("play previous video: " + link.videoFrom);
                GameManager.instance.PlayVideo(link.episode, link.videoFrom);
                return;
            }

            backButton?.OnPointerClick(null);
        }*/
    }
    
    private void Awake()
    {
        nextButton.gameObject.SetActive(false);
        nextButton.onClick.AddListener(NextLineAndClear);
        
        prevButton.gameObject.SetActive(false);
        prevButton.onClick.AddListener(PrevLineAndClear);
    }

    private void OnEnable()
    {
        foreach (var btn in clickableBackgroundButtons)
        {
            btn.onEnter += MouseEnterBackground;
            btn.onExit += MouseExitBackground;
        }
        
        foreach(var btn in choiceButtons)
        {
            btn.gameObject.SetActive(false);
            btn.userData = choiceButtons.IndexOf(btn);
            btn.onClick += ClickedChoice;
        }
    }

    private void OnDisable()
    {
        foreach (var btn in clickableBackgroundButtons)
        {
            btn.onEnter -= MouseEnterBackground;
            btn.onExit -= MouseExitBackground;
        }
        
        foreach(var btn in choiceButtons)
        {
            btn.onClick -= ClickedChoice;
        }
    }

    private void MouseEnterBackground(object userData)
    {
 //       Debug.Log("MouseEnterBackground");
        mouseOverBackground = true;
    }

    private void MouseExitBackground(object userData)
    {
//        Debug.Log("MouseExitBackground");
        mouseOverBackground = false;
    }

    private void ClickedChoice(object userData)
    {
        ChooseLink((VideoLink)userData);
        choiceButtons.ForEach(btn => btn.gameObject.SetActive(false));
    }

    private bool SpeedUpText()
    {
        return mouseOverBackground && Input.GetMouseButton(0);
    }

    private void ChooseLink(VideoLink link)
    {
        Debug.Log("ChooseLink: " + link.videoTo);
        displayContent = lastContent = string.Empty;
        choiceButtons.ForEach(btn => btn.gameObject.SetActive(false));
        nextVideo = link.videoTo;
        lineIndex = lines.Count;
        GameManager.instance.savedGame.SetLinkVisited(link.id, true);
        if (link.EpisodeComplete())
            GameManager.instance.savedGame.SetEpisodeCompleted(link.episode, true);
        if (string.IsNullOrEmpty(nextVideo))
        {
            GameManager.instance.SetCurrentPanel(GameManager.Panels.SelectEpisode);
        }
    }

    private bool cinematicMode;
    private const string CINEMATIC = "cinematique";
    IEnumerator Play()
    {
        cinematicMode = false;
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
                Debug.Log("line " + (lineIndex+1) + " / " + lines.Count);
                
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
                            cinematicMode = true;
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
//                        Debug.Log(mouseOverBackground ? "MouseOverBackground" : "MouseExitBackground");
                        yield return SpeedUpText() ? null : new WaitForSeconds(textDelay);
                    }

                    if (Skip())
                    {
                        displayContent = lastContent;
                        charIndex = lastContent.Length;
                    }
                }
                
                // text end animation

                if (cinematicMode)
                {
                    Debug.Log("Wait video end");
                    videoPlayer.isLooping = false;
                    while (videoPlayer.isPlaying)
                    {
                        if (Skip())
                            break;
                        
                        yield return null;
                    }
                    Debug.Log("Pause video");
                    videoPlayer.Pause();
                }

                if (autoPlay || cinematicMode)
                {
                    Debug.Log("autoplay next line");
                    yield return new WaitForSeconds(autoPlayDuration);
                    NextLineAndClear();
                }
                
                else if (lineIndex + 1 < lines.Count || links.Count <= 1)
                {
                    Debug.Log("show next/prev buttons");
                    nextButton.gameObject.SetActive(true);
                    prevButton.gameObject.SetActive(lineIndex - 1 >= 0);
                    while (!string.IsNullOrEmpty(lastContent)) // will be skipped if "wait video end"
                    {
                        // wait click next
                        yield return null;
                    }
                }
                
                else if(lineIndex + 1 >= lines.Count)
                {
                    if (links.Count > 1)
                    {   
                        prevButton.gameObject.SetActive(true);
                        nextButton.gameObject.SetActive(false);
                        Debug.Log("display choices: " + links.Count);
                        for (int i = 0; i < links.Count; i++)
                        {
                            var btn = choiceButtons[i];
                            btn.gameObject.SetActive(true);
                            btn.SetText(links[i].text);
                            btn.userData = links[i];
                        }
                        
                        while (choiceButtons.Any(btn => btn.gameObject.activeInHierarchy))
                        {
                            yield return null;
                        }
                    }
                    else if (links.Count > 0)
                    {
                        Debug.Log("choose first link");
                        ChooseLink(links.First()); 
                    }
                    else
                    {
                        Debug.Log(" no link");
                        nextVideo = string.Empty;
                        lineIndex = lines.Count;
                        GameManager.instance.savedGame.SetEpisodeCompleted(episode, true);
                    }
                }
                    
                nextButton.gameObject.SetActive(false);
                prevButton.gameObject.SetActive(false);
                choiceButtons.ForEach(btn => btn.gameObject.SetActive(false));
                cinematicMode = false;
                
                while (fader.isFading)
                    yield return null;
                
                yield return null;
            }

            fader.Fade1();
            while (fader.isFading)
                yield return null;

            StopVideo();
        }
        
        Debug.Log("story complete");
        storyComplete?.Invoke();
    }

    private void Update()
    {
        textBackground.enabled = !string.IsNullOrEmpty(displayContent);
    }

    private void LateUpdate()
    {
        uiText.enabled = !string.IsNullOrWhiteSpace(displayContent);
        uiText.text = displayContent;
    }
}
