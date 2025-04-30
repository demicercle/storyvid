using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class AutoplayToggle : MonoBehaviour, IPointerClickHandler
{
    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.isOn = StoryPlayer.autoPlay = PlayerPrefs.GetInt("autoplay") > 0;
        toggle.onValueChanged.AddListener((isOn) =>
        {
            StoryPlayer.autoPlay = isOn;
            PlayerPrefs.SetInt("autoplay", isOn ? 1 : 0);
            PlayerPrefs.Save();
        });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        toggle.isOn = !toggle.isOn;
    }
}
