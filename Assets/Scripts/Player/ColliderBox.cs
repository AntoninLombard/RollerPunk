using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ColliderBox : MonoBehaviour
{
    
    public enum ColliderType
    {
        Punch,
        BallPunch
    }
    [SerializeField] private Player source;
    [SerializeField] private ColliderType type;
    [SerializeField] private Collider collider;
    
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name + other.gameObject.transform.parent.name);
        Player hit = other.gameObject.transform.parent.GetComponent<Player>();
        if (hit != source)
        {
            switch (type)
            {
                // case ColliderType.Slide:
                //     hit.combat.OnHitbySlide.Invoke(source);
                //     break;
                case ColliderType.Punch:
                    hit.combat.OnHitByPunch.Invoke(source);
                    break;
                case ColliderType.BallPunch:
                    hit.combat.OnHitbByBallPunch.Invoke(source);
                    break;
                // case ColliderType.BallSlide:
                //     hit.combat.OnHitbByBallSlide.Invoke(source);
                //     break;
            }
        }
    }

    public void SetSource(Player player)
    {
        source = player;
    }

    public void SetType(ColliderType type)
    {
        this.type = type;
    }

    public void Toggle(bool isEnabled)
    {
        collider.enabled = isEnabled;
    }
}
