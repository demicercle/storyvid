using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

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
            if (lastMusic.audioSource.clip == audioClip)
            {
                //Debug.Log("music " + file + " already playing" );
                return;
            }

            StopMusic();
        }
        
        lastMusic = new GameObject("MusicPlayer (" + audioClip.name + ")").AddComponent<MusicPlayer>();
        lastMusic.isPlaying = true;
        lastMusic.audioSource.volume = 0.0f;
        lastMusic.audioSource.clip = audioClip;
        lastMusic.audioSource.loop = true;
        lastMusic.audioSource.Play();
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
    
    public float maxVolume => 1;
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup =
            Resources.Load<AudioMixer>("AudioMixer").FindMatchingGroups("Music").First();
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