using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private SphereCollider physicCollider;
    [SerializeField] private SphereCollider triggerCollider;
    [SerializeField] private float speed;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        //rb.AddForce(transform.forward * speed,ForceMode.Acceleration);
    }
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponentInParent<Player>();
        if (player != null)
        {
            //rb.isKinematic = true;
            Toggle(false);
            player.combat.OnGrabbingBall.Invoke(gameObject);
        }
    }

    public void Toggle(bool isEnabled)
    {
        physicCollider.enabled = isEnabled;
        triggerCollider.enabled = isEnabled;
    }
}
