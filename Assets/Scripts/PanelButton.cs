using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelButton : MonoBehaviour, IPointerClickHandler
{
    public GameManager.Panels panel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.instance.storyPlayer.fader.isFading)
            return;
        
        if (GameManager.instance.currentPanel == (int)GameManager.Panels.PlayVideo)
        {
            GameManager.instance.BackToMenu(panel);
        }
        else
        {
            GameManager.instance.SetCurrentPanel(panel);
        }
    }
}
