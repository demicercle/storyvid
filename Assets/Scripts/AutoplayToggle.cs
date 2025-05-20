using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoplayToggle : MonoBehaviour
{
    static public bool IsChecked
    {
        get => PlayerPrefs.GetInt("autoplay", 0) != 0;
        set 
        {
            if (value != IsChecked)
            {
                PlayerPrefs.SetInt("autoplay", value ? 1 : 0);
                PlayerPrefs.Save();
                Debug.Log("AutoplayToggle IsOn " + value);
            }
        }
    }
	
    public Toggle toggle;

    private void Awake()
    {
        toggle.onValueChanged.AddListener((isOn) =>
        {
            IsChecked = isOn;
        });
    }

    private void Start()
    {
        toggle.isOn = IsChecked;
    }
}
