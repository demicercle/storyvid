using System;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    static public float value;
    
    public Slider slider;

    private void Update()
    {
        value = slider.value;
    }
}
