using UnityEngine;
using UnityEngine.EventSystems;

public class PanelButton : MonoBehaviour, IPointerClickHandler
{
    public GameManager.Panels panel;


    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.instance.SetCurrentPanel(panel);
    }
}
