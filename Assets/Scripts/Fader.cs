using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Fader : MonoBehaviour
{
    private const float SPEED = 1f;

    public void Fade1()
    {
        targetAlpha = 1.0f;
    }

    public void Fade0()
    {
        targetAlpha = 0.0f;
    }

    public bool isFading => currentAlpha != targetAlpha;

    private Image imageComponent;
    private float currentAlpha;
    private float targetAlpha;

    private void Awake()
    {
        imageComponent = GetComponent<Image>();
        currentAlpha = targetAlpha = 1.0f;
    }

    void Update()
    {
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.unscaledDeltaTime * SPEED);
        imageComponent.color = new Color(0, 0, 0, currentAlpha);
        imageComponent.raycastTarget = currentAlpha > 0.01f;
    }
}