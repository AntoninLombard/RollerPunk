using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionSound : MonoBehaviour
{
    [SerializeField] private Player player;


    private void OnCollisionEnter(Collision other)
    {
        float force = other.impulse.magnitude;
        float vertical = Vector3.Dot(other.impulse, player.character.up);
        float remaining = force - vertical;

        if (force > player.data.collisionSoundThreshold)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Characters"))
            {
                player.data.collisionSound.Post(player.character.gameObject);
                return;
            }
            
            if (vertical > remaining)
            {
                player.data.wallHitSound.Post(player.character.gameObject);
;            }
            else
            {
                player.data.groundHitSound.Post(player.character.gameObject);
            }
        }
    }
}
