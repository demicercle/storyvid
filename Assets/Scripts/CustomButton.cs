
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CustomButton : MonoBehaviour
{
    public object userData;
    public System.Action<object> onClick;

    private Button button;

    public void SetText(string newText)
    {
        GetComponentsInChildren<TMPro.TMP_Text>().ToList().ForEach(text =>
        {
            text.text = newText;
        });
    }
    
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            onClick?.Invoke(userData);
        });
    }
}
