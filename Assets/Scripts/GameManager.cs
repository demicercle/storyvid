using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum Panels
    {
        Home,
        SelectEpisode,
        SelectVideo,
        PlayVideo
    }
    
    public int currentPanel;
    public GameObject[] panels;

    public UnityEngine.UI.Button playButton;
    public UnityEngine.UI.Button quitButton;

    private void OnDrawGizmosSelected()
    {
        UpdatePanels();
    }

    public void SetCurrentPanel(int panel)
    {
        currentPanel = panel;
    }

    private void Awake()
    {
        Localize.LoadLines();
        
        SetCurrentPanel(0);
        
        playButton.onClick.AddListener(() =>
        {
            SetCurrentPanel((int)Panels.SelectEpisode);
        });
        
        quitButton.onClick.AddListener(() =>
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        });
    }

    private void Update()
    {
        UpdatePanels();
    }

    private void UpdatePanels()
    {
        currentPanel = Mathf.Clamp(currentPanel, 0, panels.Length - 1);
        for (int i = 0; i < panels.Length; ++i)
        {
            panels[i].SetActive(i == currentPanel);
        }
    }
}
