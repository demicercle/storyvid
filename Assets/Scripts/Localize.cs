using System;
using TMPro;
using UnityEngine;

public class Localize : MonoBehaviour
{
    public string id;

    private void OnDrawGizmosSelected()
    {
        id = id.ToLower();
        UpdateText();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
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
        var textComponent = GetComponent<TMPro.TMP_Text>();
        if (textComponent != null)
        {
            textComponent.text = Application.isPlaying ? GameManager.instance.GetLocalizedText(id) : id;
        }
    }
}
