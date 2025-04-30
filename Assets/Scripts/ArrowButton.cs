using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArrowButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public ScrollRect scrollRect;
    public float speed;

    private bool mouseDown;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        mouseDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mouseDown = false;
    }

    private void Update()
    {
        if (mouseDown)
            scrollRect.horizontalNormalizedPosition += speed * Time.deltaTime;
    }
}
