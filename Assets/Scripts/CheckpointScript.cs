using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void OnBallHit(GameManager player)
    {
        particles.Play();
        
        //GameManager.Instance; // TODO notice the Manager 
    }

    public void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<PlayerController>()?.onDeath(null);
        //TODO notice manager of player collision
    }
}
