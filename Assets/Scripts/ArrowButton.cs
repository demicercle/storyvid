using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArrowButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public CustomScrollview scrollView;
    public float speed;
    public float speedScale;

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
            scrollView.MoveHorizontal(speedScale * speed * Time.deltaTime);
    }
}
