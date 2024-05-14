using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private List<Transform> spawnPoints;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.transform.parent?.gameObject.GetComponent<Player>();
        if(player == GameManager.Instance.ballHolder)
        {
            GameManager.Instance.ChangeLastCheckPoint(this);
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        Debug.Log("CheckPoint Collision");
        Player player = other.transform.parent?.gameObject.GetComponent<Player>();
        if(player == GameManager.Instance.ballHolder)
        {
            player?.combat.onDeath(null);
            GameManager.Instance.OnScoring();
        }
    }
}
