using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;


public class Logo : MonoBehaviour
{
    public AudioListener audioListener;
    public VideoClip logoClip;
    public VideoPlayer videoPlayer;
    public Graphic fader;
    public float fadeDuration = 1f;
    public int sceneToLoad = 1;
    
    IEnumerator Start()
    {
        Cursor.visible = false;
        Debug.Log(this + " start");
        MusicPlayer.PlayMusic("musiquelogo");
        DontDestroyOnLoad(gameObject);
        fader.DOFade(0, fadeDuration);
        videoPlayer.clip = logoClip;
        videoPlayer.isLooping = false;
        videoPlayer.Play();
        Debug.Log(this + " logoClip=" + logoClip.name + " (" + logoClip.length + ")");
        yield return new WaitForSeconds((float)logoClip.length - fadeDuration);
        Debug.Log(this + " fade out for loading");
        fader.DOFade(1, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
        videoPlayer.enabled = false;
        Destroy(audioListener);
        Debug.Log(this + " load main scene");
        var asyncOp = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        while (!asyncOp.isDone)
            yield return null;
        Debug.Log(this + " fade and destroy");
        fader.DOFade(0, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
        Cursor.visible = true;
        SceneManager.UnloadSceneAsync(0);
    }
}