using System;
using System.Collections;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    static public float fadeSpeed = 1.0f;
    static private MusicPlayer lastMusic;

    static public void PlayMusic(string file)
    {
        var audioClip = Resources.Load<AudioClip>("Music/" + file);
        if (audioClip == null)
        {
            Debug.LogError("Unknown music file: " + file);
            return;
        }

        if (lastMusic != null)
        {
            if (lastMusic.audioClip == audioClip)
            {
                Debug.Log("music " + file + " already playing" );
                return;
            }

            StopMusic();
        }
        
        lastMusic = new GameObject("MusicPlayer (" + file + ")").AddComponent<MusicPlayer>();
        lastMusic.isPlaying = true;
        lastMusic.audioClip = audioClip;
        Debug.Log("PlayMusic: " + audioClip);
    }

    static public void StopMusic()
    {
        if (lastMusic != null)
        {
            lastMusic.isPlaying = false;
            lastMusic = null;
        }
    }

    public bool isPlaying;
    public AudioSource audioSource;
    public AudioClip audioClip;

    public float maxVolume => VolumeSlider.value;
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        audioSource.volume = 0.0f;
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    void Update()
    {
        if (isPlaying)
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, maxVolume, Time.deltaTime * fadeSpeed);
        }
        else
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0.0f, Time.deltaTime * fadeSpeed);
            if (audioSource.volume == 0.0f)
            {
                Destroy(gameObject);
                Resources.UnloadUnusedAssets();
            }
        }
    }
}