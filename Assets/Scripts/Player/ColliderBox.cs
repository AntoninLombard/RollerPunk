using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ColliderBox : MonoBehaviour
{
    
    // public enum ColliderType
    // {
    //     Punch,
    //     BallPunch
    // }
    [SerializeField] private Player source;
    //[SerializeField] private ColliderType type;
    [SerializeField] private Collider collider;
    
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name + other.gameObject.transform.parent.name);
        Player hit = other.gameObject.GetComponentInParent<Player>();
        if (hit != source)
        {
            hit.combat.OnHitByPunch.Invoke(source);
        }
    }

    public void SetSource(Player player)
    {
        source = player;
    }

    // public void SetType(ColliderType type)
    // {
    //     this.type = type;
    // }

    public void Toggle(bool isEnabled)
    {
        collider.enabled = isEnabled;
    }
}
