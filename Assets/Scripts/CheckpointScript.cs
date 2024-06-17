using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private float shredDuration;
    [SerializeField] private VisualEffect bloodRainVFX;
    [SerializeField] private VisualEffect bloodShowerVFX;
    [SerializeField] private VisualEffect bloodCanonBurst1VFX;
    [SerializeField] private VisualEffect bloodCanonBurst2VFX;
    [SerializeField] private VisualEffect canonShotFX;

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
            StartCoroutine(BallHolderShred());
        }
        else
        {
            player.combat.Kill(null);
            StartCoroutine(PlayerShred());
        }
    }


    private IEnumerator BallHolderShred()
    {
        yield return PlayerShred();
        bloodCanonBurst1VFX.Play();
        bloodCanonBurst2VFX.Play();
        canonShotFX.Play();
        bloodRainVFX.Play();
    }
    
    private IEnumerator PlayerShred()
    {
        bloodShowerVFX.Play();
        yield return new WaitForSeconds(shredDuration);
        bloodShowerVFX.Stop();
    }
}
