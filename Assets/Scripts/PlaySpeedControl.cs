using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaySpeedControl : MonoBehaviour
{
    public TMPro.TMP_Dropdown dropdown;
    public List<Toggle> toggles = new List<Toggle>();
    public ToggleGroup toggleGroup;
    
    private void Start()
    {
        if (dropdown != null)
        {
            dropdown.ClearOptions();

            dropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
            {
                new TMP_Dropdown.OptionData("1"),
                new TMP_Dropdown.OptionData("2"),
                new TMP_Dropdown.OptionData("3"),
            });

            dropdown.value = StoryPlayer.selectedSpeed;
            dropdown.onValueChanged.AddListener((value) =>
            {
                StoryPlayer.selectedSpeed = value;
            });
        }

        for (int i = 0; i < toggles.Count; i++)
        {
            toggles[i].onValueChanged.AddListener(ToggleValueChanged);
            toggles[i].group = toggleGroup;
            toggles[i].group.allowSwitchOff = false;
            toggles[i].isOn = i == StoryPlayer.selectedSpeed;
            toggles[i].GetComponentInChildren<TMPro.TMP_Text>().text = (i+1).ToString();
        }
    }

    void ToggleValueChanged(bool value)
    {
        if (value)
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                if (toggles[i].isOn)
                {
                    StoryPlayer.selectedSpeed = i;
                    break;
                }
            }
        }
    }
}