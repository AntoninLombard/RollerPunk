using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    { 
        Player hit = other.gameObject.GetComponentInParent<Player>();
        if (hit != null)
        {
            hit.combat.Fall();
        }
    }
}
