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
    [SerializeField] private GameObject source;
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
        if (other.gameObject != source)
        {
            PlayerController pc = other.gameObject.GetComponentInParent<PlayerController>();
            if (pc != null)
            {
                switch (type)
                {
                    case ColliderType.Slide:
                        pc.OnHitbySlide.Invoke(source);
                        break;
                    case ColliderType.Punch:
                        pc.OnHitByPunch.Invoke(source);
                        break;
                    case ColliderType.BallPunch:
                        pc.OnHitbByBallPunch.Invoke(source);
                        break;
                    case ColliderType.BallSlide:
                        pc.OnHitbByBallSlide.Invoke(source);
                        break;
                }
            }
        }
    }

    public void SetSource(GameObject gameObject)
    {
        source = gameObject;
    }

    public void SetType(ColliderType type)
    {
        this.type = type;
    }
}
