using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;

    [SerializeField] private AK.Wwise.Event scoring;
    
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("CheckPoint Collision");
        Player player = other.transform.parent?.gameObject.GetComponent<Player>();
        if (player == null)
            return;
        if(player == GameManager.Instance.ballHolder)
        {
            scoring.Post(gameObject);
            GameManager.Instance.Scoring();
            player.combat.Kill(null);
        }
        else
        {
            player.combat.Kill(null);
        }
    }
}
