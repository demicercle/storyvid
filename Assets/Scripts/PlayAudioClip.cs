using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


public class PlayAudioClip : MonoBehaviour, IPointerClickHandler
{
    public AudioClip clip;
    public bool playOnClick;
    
    public void PlaySound()
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, FindObjectsByType<AudioListener>(FindObjectsSortMode.None).FirstOrDefault().transform.position);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (playOnClick)
            PlaySound();
    }
}