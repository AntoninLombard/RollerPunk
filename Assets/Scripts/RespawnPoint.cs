using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField] public bool isBallRespawn;
    [field: SerializeField] public List<Transform> spawnPoints { get; private set; }
    [field: SerializeField] public Transform ballSpawnPoint { get; private set; }
    private void OnTriggerEnter(Collider other)
    {
        Player hit = other.gameObject.GetComponentInParent<Player>();
        if (hit!= null && hit == GameManager.Instance.ballHolder && GameManager.Instance.lastRespawnPoint != this)
        {
            GameManager.Instance.ChangeLastCheckPoint(this);
        }
    }
}
