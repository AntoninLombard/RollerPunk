using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class OnSelectButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public UnityEvent onSelectEvent;
    public UnityEvent onDeselectEvent;
    
    public void OnSelect(BaseEventData eventData)
    {
        onSelectEvent.Invoke();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        onDeselectEvent.Invoke();
    }
}
