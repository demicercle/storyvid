using System;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionControl : MonoBehaviour
{
    public TMPro.TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Vector2Int[] resolutions;
    
    private void Awake()
    {
        resolutionDropdown.ClearOptions();
        foreach (Vector2Int resolution in resolutions)
            resolutionDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(resolution.ToString().Trim('(',')')));
        resolutionDropdown.onValueChanged.AddListener((option) =>
        {
            UpdateResolution();
        });

        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener((isOn) =>
        {
            UpdateResolution();
        });
    }

    private void UpdateResolution()
    {
        Screen.SetResolution(resolutions[resolutionDropdown.value].x, resolutions[resolutionDropdown.value].y, fullscreenToggle.isOn);
    }
}
