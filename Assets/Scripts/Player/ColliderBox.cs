using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ColliderBox : MonoBehaviour
{
    
    public enum ColliderType
    {
        Slide,
        Punch,
        BallPunch,
        BallSlide
    }
    [SerializeField] private Player source;
    [SerializeField] private ColliderType type;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name + other.gameObject.transform.parent.name);
        Player hit = other.gameObject.transform.parent.GetComponent<Player>();
        if (hit != source)
        {
            switch (type)
            {
                case ColliderType.Slide:
                    hit.controller.OnHitbySlide.Invoke(source);
                    break;
                case ColliderType.Punch:
                    hit.controller.OnHitByPunch.Invoke(source);
                    break;
                case ColliderType.BallPunch:
                    hit.controller.OnHitbByBallPunch.Invoke(source);
                    break;
                case ColliderType.BallSlide:
                    hit.controller.OnHitbByBallSlide.Invoke(source);
                    break;
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
}
