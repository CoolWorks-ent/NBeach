using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PointerEventDataExtra : PointerEventData
{
    public bool collide;
    public PointerEventData pointerData;

    public PointerEventDataExtra(EventSystem eventSystem) : base(eventSystem)
    {
        collide = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        collide = true;
        pointerData = eventData;
    }
}
