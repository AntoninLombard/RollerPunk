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
        rb.AddForce(transform.forward * speed,ForceMode.Acceleration);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.gameObject.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            rb.isKinematic = true;
            physicCollider.enabled = false;
            triggerCollider.enabled = false;
            player.OnGrabbingBall.Invoke(gameObject);
        }
    }
    
    
}
