using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class WwiseEventManager : MonoBehaviour
{
    [Header("Wwise Events")]
    [SerializeField] private AK.Wwise.Event[] events;

    public void PlayWwiseEvent(int i)
    {
        if (events != null && i < events.Length && events[i] != null)
        {
            events[i].Post(gameObject); 
        }
    }
}
