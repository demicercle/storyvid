using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleObjects : MonoBehaviour
{
    public Toggle toggle;
    public List<GameObject> objectsOn = new List<GameObject>();
    public List<GameObject> objectsOff = new List<GameObject>();

    private void Awake()
    {
        toggle.onValueChanged.AddListener((isOn) =>
        {
            foreach (var obj in objectsOn) obj.SetActive(isOn);
            foreach (var obj in objectsOff) obj.SetActive(!isOn);
        });
    }
}
