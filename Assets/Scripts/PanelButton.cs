using UnityEngine;
using UnityEngine.EventSystems;

public class PanelButton : MonoBehaviour, IPointerClickHandler
{
    public GameManager.Panels panel;

    public void SetPanel(int newPanel)
    {
        panel = (GameManager.Panels)newPanel;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.instance.SetCurrentPanel(panel);
    }
}
