using System;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    static public float value { get; private set; }
    
    public Slider slider;

    private void Awake()
    {
        value = PlayerPrefs.GetFloat("Volume", 0.8f);
        slider.onValueChanged.AddListener((newValue) =>
        {
            value = newValue;
            PlayerPrefs.SetFloat("Volume", value);
            PlayerPrefs.Save();
        });
    }
}
