using System;
using UnityEngine;

public class Localize : MonoBehaviour
{
    static private string[] lines;

    static public void LoadLines()
    {
        var textAsset = Resources.Load<TextAsset>("dialog");
        if (textAsset != null)
            lines = textAsset.text.Split('\n');
    }
    
    static public string GetLine(int index)
    {
        if (lines == null)
            LoadLines();
        
        if (lines != null && index >= 0 && index < lines.Length)
            return lines[index];
        else
            return string.Empty;
    }

    public int lineIndex;

    private void OnDrawGizmosSelected()
    {
        Awake();
    }

    private void Awake()
    {
        var textComponent = GetComponent<TMPro.TMP_Text>();
        if (textComponent != null)
        {
            textComponent.text = GetLine(lineIndex);
        }
    }
}
