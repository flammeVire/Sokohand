using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResetButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isClicked = false;
    [SerializeField] UIManager manager;
    public void OnPointerDown(PointerEventData eventData)
    {
        isClicked = true;
        manager.ResetButtonUI(isClicked);
    }
    // Cette m�thode est appel�e lorsque le bouton est rel�ch�
    public void OnPointerUp(PointerEventData eventData)
    {
        isClicked = false;
        manager.ResetButtonUI(isClicked);
    }
}
