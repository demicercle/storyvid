using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Fader : MonoBehaviour
{
    private const float SPEED = 1f;

    public void Fade1(float speed = SPEED, System.Action callback=null)
    {
        imageComponent.DOComplete();
        tween = imageComponent.DOFade(1f, speed)
            .OnComplete(() => callback?.Invoke());
    }

    public void Fade0(float speed = SPEED, System.Action callback=null)
    {
        imageComponent.DOComplete();
        tween = imageComponent.DOFade(0f, speed)
            .OnComplete(() => callback?.Invoke());
    }

    public bool isFading => tween != null && tween.IsActive();

    private Image imageComponent;
    private Tween tween;

    private void Awake()
    {
        imageComponent = GetComponent<Image>();
    }

    private void Update()
    {
        imageComponent.raycastTarget = isFading;
    }
}