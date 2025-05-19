using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


public class PlayAudioClip : MonoBehaviour, IPointerClickHandler
{
    public AudioClip clip;
    public bool playOnClick;

    private AudioSource audioSource;
    
    public void PlaySound()
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (playOnClick)
            PlaySound();
    }

    void Awake()
    {
        audioSource = GameObject.Find("AudioSourceUI").GetComponent<AudioSource>();
    }
}