using System;
using UnityEngine;
using UnityEngine.UI;


public class Popup : MonoBehaviour
{
    public GameObject container;
    public Button yesButton, noButton;

    public void DisplayYesNo(System.Action yesAction, System.Action noAction = null)
    {
        container.SetActive(true);
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => { yesAction?.Invoke(); container.SetActive(false); });
        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(() => { noAction?.Invoke(); container.SetActive(false); });
    }

    private void Awake()
    {
        container.SetActive(false);
    }
}