using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardShortcut : MonoBehaviour
{
    public Button button;
    public List<KeyCode> keyCodes;

    private void Update()
    {
        if (keyCodes.Any(k => Input.GetKeyDown(k)))
        {
            button.onClick?.Invoke();
        }
    }
}