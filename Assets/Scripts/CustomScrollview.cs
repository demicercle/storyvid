using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomScrollview : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public ScrollRect scrollRect;
    
    public void OnDrag(PointerEventData eventData)
    {
        scrollRect.horizontalNormalizedPosition += eventData.delta.x / Screen.width;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}