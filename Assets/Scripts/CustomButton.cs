
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public object userData;
    public System.Action<object> onClick, onPress, onRelease, onEnter, onExit;

    public void SetText(string newText)
    {
        GetComponentsInChildren<TMPro.TMP_Text>().ToList().ForEach(text =>
        {
            text.text = newText;
        });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(userData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPress?.Invoke(userData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onRelease?.Invoke(userData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke(userData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onExit?.Invoke(userData);
    }
}
