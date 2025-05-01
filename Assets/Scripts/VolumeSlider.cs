using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider slider;
    public AudioMixer mixer;
    public AnimationCurve curve;

    private float value;

    private void UpdateMixer()
    {
        mixer.SetFloat("Volume", curve.Evaluate(value));
    }
    
    private void Start()
    {
        value = PlayerPrefs.GetFloat("Volume", 0.8f);
        UpdateMixer();
        if (slider != null)
        {
            slider.onValueChanged.AddListener((newValue) =>
            {
                value = newValue;
                UpdateMixer();
                PlayerPrefs.SetFloat("Volume", value);
                PlayerPrefs.Save();
            });
        }
    }
}
