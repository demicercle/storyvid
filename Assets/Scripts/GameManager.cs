using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Serialization;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    static public GameManager instance { get; private set; }
    
    public enum Panels
    {
        Home,
        SelectEpisode,
        SelectVideo,
        PlayVideo,
        Options,
        Concept
    }

    public StoryPlayer storyPlayer;
    public GameObject[] panels;
    public VideoPlayer menuVideoPlayer;
    
    public string language { get; private set; }

   static public System.Action onLanguageChanged;
    public void SetLanguage(string key)
    {
        language = key;
        onLanguageChanged?.Invoke();
    }

    public Popup popup;
    public UnityEngine.UI.Button playButton;
    public UnityEngine.UI.Button quitButton;
    public UnityEngine.UI.Button deleteGameButton;
    
    public TextAsset jsonAsset;
    public Newtonsoft.Json.Linq.JObject jsonData;
    public List<VideoLink> videoLinks;
    public List<MusicLink> musicLinks;
    
    public int currentPanel { get; private set; }

    public void SetCurrentPanel(Panels panel)
    {
        currentPanel = (int)panel;
        UpdatePanels();
    }

    public void SetCurrentPanel(int panel)
    {
        currentPanel = panel;
        UpdatePanels();
    }

    public bool GetLastUnlockedVideo(int episode, out string videoID)
    {
        videoID = GetVideoIDs(episode).LastOrDefault(id => savedGame.IsPathUnlocked(episode, id));
        return !string.IsNullOrEmpty(videoID);
    }

    public string GetVideoID(int video)
    {
        return "video" + (video < 10 ? "0" : "") + video;
    }
    
    public void PlayEpisode(int episode)
    {
        PlayVideo(episode, GetVideoIDs(episode).FirstOrDefault());
    }
    
    public void PlayVideo(int episode, string videoID)
    {
        Debug.Log(this + " PlayVideo: " + episode + " - " + videoID);
        var content = GetVideoContent(episode, videoID);
        storyPlayer.lines.Clear();
        storyPlayer.lines.AddRange(content.Replace("\r", "\n").Split('\n'));
        storyPlayer.lines = storyPlayer.lines.Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
        storyPlayer.links = GetLinks(videoID);
        if (storyPlayer.links.Count == 1 && string.IsNullOrWhiteSpace(storyPlayer.links.First().text))
        {
            storyPlayer.nextVideo = storyPlayer.links[0].videoTo;
        }
        else
        {
            storyPlayer.nextVideo = string.Empty;
        }
        storyPlayer.episode = episode;
        storyPlayer.musicFile = GetMusicFile(episode, videoID);
        var videoFile = GetVideoFile(episode, videoID);
        savedGame.UnlockPath(episode, videoID);
        playedFromGallery = currentPanel == (int)Panels.SelectVideo;
        storyPlayer.backButton.panel = playedFromGallery ? Panels.SelectVideo : Panels.SelectEpisode;
        if ((Panels)currentPanel != Panels.PlayVideo)
        {
            storyPlayer.fader.Fade1(1f, () =>
            {
                storyPlayer.PlayVideo(videoFile);
                SetCurrentPanel((int)Panels.PlayVideo);
            });
        }
        else
        {
            storyPlayer.PlayVideo(videoFile);
        }
    }

    public void StopVideo()
    {
        storyPlayer.StopVideo();
        MusicPlayer.StopMusic();
        SetCurrentPanel(Panels.SelectEpisode);
    }

    public string GetMusicFile(int episode, string videoID)
    {
        return musicLinks.FirstOrDefault(m => m.episode == episode && m.videoFrom == videoID).musicFile;
    }

    public bool GetImageForVideo(out Texture texture, int episode, string videoID = null)
    {
        texture = null;
        if (string.IsNullOrEmpty(videoID))
            videoID = GetVideoIDs(episode).FirstOrDefault();
        if (!string.IsNullOrEmpty(videoID))
            texture = Resources.Load<Texture>("Images/" + GetVideoImageFile(episode, videoID));
        return texture != null;
    }

    public string[] GetVideoIDs(int episode)
    {
        var keys = new List<string>();

        if (jsonData.ContainsKey("episode" + episode))
        {
            foreach (JProperty jProp in (jsonData["episode" + episode] as JObject).Properties())
            {
                keys.Add(jProp.Name.ToString());
            }
        }

        return keys.ToArray();
    }

    private void ParseLinks()
    {
        videoLinks.Clear();
        
        var linksData = jsonData["links"] as JArray;
        foreach (var data in linksData)
        {
            var link = new VideoLink();
            link.id = int.Parse(data["id"].ToString());
            link.episode = int.Parse(data["episode"].ToString());
            link.videoFrom = data["video_from"].ToString();
            link.videoTo = data["video_to"].ToString();
            link.completeEpisode = data["complete_episode"].ToString().ToLower() == "true";
            link.text = data["text_" + language].ToString();
            if (int.TryParse(data["points"].ToString(), out var pts))
                link.points = pts;
            link.name = link.ToString();
            videoLinks.Add(link);
        }
    }

    private void ParseMusics()
    {
        musicLinks.Clear();

        var musicsData = jsonData["musics"] as JArray;
        foreach (var data in musicsData)
        {
            var music = new MusicLink();
            music.episode = int.Parse(data["episode"].ToString());
            music.videoFrom = data["video"].ToString();
            music.musicFile = data["music"].ToString();
            musicLinks.Add(music);
        }
    }
    
    [ContextMenu("Parse JSON")]
    private void ParseJson()
    {
        Debug.Log("ParseJson:" + jsonAsset.text);
        jsonData = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(jsonAsset.text);
        ParseLinks();
        ParseMusics();
    }

    public string GetLocalizedText(string key)
    {
        var jo = (jsonData["interface"] as JObject);
        if (!jo.ContainsKey(key))
        {
            Debug.LogError("localize key not found: " + key);
        }
        jo = jo[key] as JObject;
        if (!jo.ContainsKey("text_" + language))
        {
            Debug.LogError("localized language " + language + " not found in key: " + key);
        }
        return jo["text_" + language].ToString();
    }

    public JObject GetVideoData(int episode, string videoID)
    {
        var episodeData = jsonData["episode" + episode] as JObject;
        if (episodeData != null)
            return (episodeData[videoID] as JObject);
        else
            return null;
    }

    public string GetVideoImageFile(int episode, string videoID)
    {
        var videoData = GetVideoData(episode, videoID);
        if (videoData != null)
            return videoData["image_file"].ToString();
        else
            return string.Empty;
    }

    public string GetVideoFile(int episode, string videoID)
    {
        var videoData = GetVideoData(episode, videoID);
        if (videoData != null)
            return videoData["video_file"].ToString();
        else
            return string.Empty;
    }

    public string GetVideoContent(int episode, string videoID)
    {
        var videoData = GetVideoData(episode, videoID);
        if (videoData != null)
            return videoData["script_" + language].ToString();
        else
            return string.Empty;
    }

    public List<VideoLink> GetLinks(string videoFrom)
    {
        return videoLinks.Where(link => link.videoFrom == videoFrom).ToList();
    }

    public List<VideoLink> GetPreviousLinks(string videoTo)
    {
        return videoLinks.Where(link => link.videoTo == videoTo).ToList();
    }

    public SavedGame savedGame { get; private set; }
    public bool playedFromGallery { get; private set; }
    private void Awake()
    {
        instance = this;
        language = "fr";
        
        ParseJson();
        
        savedGame = new SavedGame();
        savedGame.Load();
        
        SetCurrentPanel(0);
    }

    private void OnEnable()
    {
        storyPlayer.storyComplete += StoryComplete;
        playButton.onClick.AddListener(ClickSelectEpisode);
        quitButton.onClick.AddListener(QuitGame);
        deleteGameButton.onClick.AddListener(ClickDeleteGame);
    }

    private void OnDisable()
    {
        storyPlayer.storyComplete -= StoryComplete;
        playButton.onClick.RemoveListener(ClickSelectEpisode);
        quitButton.onClick.RemoveListener(QuitGame);
        deleteGameButton.onClick.RemoveListener(ClickDeleteGame);
    }

    private void ClickDeleteGame()
    {
        popup.DisplayYesNo(() =>
        {
            savedGame.DeleteSave();
            savedGame = new SavedGame();
        });
    }

    private void ClickSelectEpisode()
    {
        SetCurrentPanel((int)Panels.SelectEpisode);
    }

    private void StoryComplete()
    {
        Debug.Log(this + " StoryComplete() nextVideo=" + storyPlayer.nextVideo + " playedFromGallery=" + playedFromGallery);
        if (!playedFromGallery && !string.IsNullOrEmpty(storyPlayer.nextVideo))
        {
            PlayVideo(storyPlayer.episode, storyPlayer.nextVideo);
        }
        else
        {
            StopVideo();
            BackToMenu(playedFromGallery ? Panels.SelectVideo : Panels.SelectEpisode);
        }
    }

    private void QuitGame()
    {
        storyPlayer.fader.Fade1(1f, () =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        });
    }

    private IEnumerator Start()
    {
        UpdatePanels();
        
        storyPlayer.fader.Fade1(0f);
        Debug.Log("Prepare Menu Video");
        menuVideoPlayer.Prepare();
        
        do
        {
            yield return new WaitForSeconds(1.5f);
        } while (!menuVideoPlayer.isPrepared);
        
        Debug.Log("Play Menu Video");
        menuVideoPlayer.Play();
        
        do
        {
            yield return new WaitForSeconds(1.5f);
        } while (menuVideoPlayer.time <= 0.5);
        
        Debug.Log("Set Panel and Fade Out");
        storyPlayer.fader.Fade0();
    }

    private void UpdatePanels()
    {
        currentPanel = Mathf.Clamp(currentPanel, 0, panels.Length - 1);

        for (int i = 0; i < panels.Length; ++i)
        {
            panels[i].SetActive(i == currentPanel);
        }
        
        if (currentPanel != (int)Panels.PlayVideo)
        {
            MusicPlayer.PlayMusic("musiquemenu");
        }
        else
        {
            Debug.Log("Stop Menu Video");
            menuVideoPlayer.Stop();
        }
        
        
        Debug.Log("GetPoints: " + savedGame.GetPoints());
    }

    public void BackToMenu(Panels newPanel)
    {
        currentPanel = (int)newPanel;
        StopCoroutine("BackToMenuCoroutine");
        StartCoroutine("BackToMenuCoroutine");
    }

    private IEnumerator BackToMenuCoroutine()
    {
        var fader = storyPlayer.fader;

        Debug.Log("BackToMenuCoroutine: fade out: " + fader.fade1);
        if (!fader.fade1)
        {
            fader.Fade1();
            while (fader.isFading) yield return null;
        }

        Debug.Log("Stop story player");
        storyPlayer.StopVideo();
        
        UpdatePanels();
            
        Debug.Log("Prepare Menu Video");
        menuVideoPlayer.Prepare();
        while (!menuVideoPlayer.isPrepared)
            yield return null;

        Debug.Log("Play Menu Video");
        menuVideoPlayer.Play();
        
        fader.Fade0();
        while (storyPlayer.fader.isFading)
            yield return null;
    }
    
    private void LateUpdate()
    {
        if (savedGame.changed)
        {
            savedGame.Save();
        }
    }

    public List<string> availableLanguages = new List<string>();
    public bool IsLanguageAvailable(string id)
    {
        return availableLanguages.Contains(id);
    }

    public string GetEpisodeHeader(int episodeIndex)
    {
        var header = GetLocalizedText("episode"  + episodeIndex);
        if (string.IsNullOrEmpty(header))
            header = "Episode " + episodeIndex;
        return header;
    }
}
