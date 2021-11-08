using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class HoverableUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        SelectionHandler.DisableSelection();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SelectionHandler.EnableSelection();
    }
}