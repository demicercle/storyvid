using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlaySpeedControl : MonoBehaviour
{
    public TMPro.TMP_Dropdown dropdown;

    private void Start()
    {
        dropdown.ClearOptions();

        dropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
        {
            new TMP_Dropdown.OptionData(GameManager.instance.GetLocalizedText("speed0")),
            new TMP_Dropdown.OptionData(GameManager.instance.GetLocalizedText("speed1")),
            new TMP_Dropdown.OptionData(GameManager.instance.GetLocalizedText("speed2")),
        });

        dropdown.value = StoryPlayer.selectedSpeed;
        dropdown.onValueChanged.AddListener((value) =>
        {
            StoryPlayer.selectedSpeed = value;
        });
    }
}