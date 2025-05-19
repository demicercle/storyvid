using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoplayToggle : MonoBehaviour, IPointerClickHandler
{
    static public bool IsOn
    {
        get => PlayerPrefs.GetInt("autoplay", 0) == 1;
        set 
        {
            if (value != IsOn)
            {
                PlayerPrefs.SetInt("autoplay", IsOn ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
    }


	
    public Toggle toggle;

    private void Awake()
    {
        toggle.isOn = IsOn;
        toggle.onValueChanged.AddListener((isOn) => IsOn = isOn);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        toggle.isOn = !toggle.isOn;
    }
}
