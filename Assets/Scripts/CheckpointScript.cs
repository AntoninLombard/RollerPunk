using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;

    [SerializeField] private AK.Wwise.Event scoring;
    
    public void OnCollisionEnter(Collision other)
    {
        Debug.Log("CheckPoint Collision");
        Player player = other.transform.parent?.gameObject.GetComponent<Player>();
        if(player == GameManager.Instance.ballHolder)
        {
            player?.combat.onDeath(null);
            scoring.Post(gameObject);
            GameManager.Instance.OnScoring();
        }
    }
}
