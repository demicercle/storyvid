using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class LanguageButton : MonoBehaviour
{
    private static ToggleGroup toggleGroup;
    
    public string id;
    
    void Start()
    {
        if (toggleGroup == null)
        {
            toggleGroup = new GameObject("LanguageToggle").AddComponent<ToggleGroup>();
            toggleGroup.allowSwitchOff = false;
        }
        
        var btn = GetComponent<Toggle>();
        btn.group = toggleGroup;
        btn.isOn = GameManager.instance.language == id;
        btn.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
                GameManager.instance.SetLanguage(id);
        });
    }
}