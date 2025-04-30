using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LanguageButton : MonoBehaviour
{
    public Toggle toggle;
    public ToggleGroup group;
    public string id;
    
    void Start()
    {
        if (group == null)
            group = GetComponentInParent<ToggleGroup>();
        
        toggle.interactable = GameManager.instance.IsLanguageAvailable(id);
        toggle.group = group;
        toggle.isOn = GameManager.instance.language == id;
        toggle.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
                GameManager.instance.SetLanguage(id);
        });
    }
}