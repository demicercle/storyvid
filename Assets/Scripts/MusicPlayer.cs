using System;
using System.Collections;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    static public float fadeSpeed = 0.6f;
    static private MusicPlayer lastMusic;

    static public void PlayMusic(string file)
    {
        var audioClip = Resources.Load<AudioClip>("Music/" + file);
        if (audioClip == null)
        {
            Debug.LogError("Unknown music file: " + file);
            return;
        }

        if (lastMusic != null && lastMusic.audioClip == audioClip)
        {
            return;
        }
        
        StopMusic();
        lastMusic = new GameObject("MusicPlayer (" + file + ")").AddComponent<MusicPlayer>();
        lastMusic.isPlaying = true;
        lastMusic.audioClip = audioClip;
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
    
    void Awake()
    {
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
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, 1.0f, Time.deltaTime * fadeSpeed);
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