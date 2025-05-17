using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Localize : MonoBehaviour
{
    public string id;

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            id = id.ToLower();
            if (TryGetComponent<TMPro.TMP_Text>(out var tmp))
            {
                tmp.text = id;
            }
            else if (TryGetComponent<Text>(out var text))
            {
                text.text = id;
            }
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        }
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
        if (TryGetComponent<TMPro.TMP_Text>(out var tmp))
        {
            tmp.text = GameManager.instance.GetLocalizedText(id);
        }
        else if (TryGetComponent<Text>(out var text))
        {
            text.text = GameManager.instance.GetLocalizedText(id);
        }
        
        GetComponent<RectTransform>().ForceUpdateRectTransforms();
    }
}
