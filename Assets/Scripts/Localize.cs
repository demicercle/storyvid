using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Localize : MonoBehaviour
{
    public string id;
    
    private TMPro.TMP_Text tmpText;
    private TMPro.TMP_FontAsset defaultFont;

    private void Awake()
    {
        tmpText = GetComponent<TMPro.TMP_Text>();
        defaultFont = tmpText.font;
    }

    private void OnDisable()
    {
        GameManager.onLanguageChanged -= UpdateText;
    }

    private void OnEnable()
    {
        UpdateText();
        GameManager.onLanguageChanged += UpdateText;
    }

    private void UpdateText()
    {
        tmpText.font = GameManager.instance.GetLanguageTMPFontAsset(out var newFont) ? newFont : defaultFont;
        tmpText.text = GameManager.instance.GetLocalizedText(id);
        GetComponent<RectTransform>().ForceUpdateRectTransforms();
    }
}
