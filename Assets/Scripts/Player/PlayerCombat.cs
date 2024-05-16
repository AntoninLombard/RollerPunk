using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Player player;
    
    [Header("Player Inputs")] 
    [SerializeField] private bool punchInput;
    [SerializeField] private bool slideInput;
    [SerializeField] private bool fortifyInput;
    

    [field: Header("CHARACTER STATE")]
    [field: SerializeField] public bool isInvincible { get; private set; }
    [field: SerializeField] public bool isRecovering { get; private set; }
    [field: SerializeField] public bool isWindingUpPunchLeft { get; private set; }
    [field: SerializeField] public bool isWindingUpPunchRight { get; private set; }
    [field: SerializeField] public bool isPunchingLeft { get; private set; }
    [field: SerializeField] public bool isPunchingRight { get; private set; }
    [field: SerializeField] public bool isTaunting { get; private set; }
    [field: SerializeField] public bool isFortified { get; private set; }
    [field: SerializeField] public bool isSliding { get; private set; }
    [field: SerializeField] public bool isStunned { get; private set; }
    [field: SerializeField] public bool isHoldingBall { get; private set; }
    public bool isBusy => isPunchingLeft || isPunchingRight || isRecovering || isFortified || isStunned || isTaunting || isWindingUpPunchLeft || isWindingUpPunchRight;
    public bool isPunching => isPunchingLeft || isPunchingRight;
    public bool isWindingUpPunch => isWindingUpPunchLeft || isWindingUpPunchRight;

    [Header("Events")] 
    [SerializeField] public UnityEvent<Player> OnHitByPunch;
    [SerializeField] public UnityEvent<Player> OnHitbByBallPunch;
    [SerializeField] public UnityEvent OnGrabbingBall;
    
    
    [Header("Parts")] 
    [SerializeField] private Transform ballAnchorPoint;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private ColliderBox punchLeftCollider;
    [SerializeField] private ColliderBox punchRightCollider;
    
    
    private void Awake()
    {
        OnHitByPunch.AddListener(onHitByPunch);
        OnHitbByBallPunch.AddListener(onHitByBallPunch);
        OnGrabbingBall.AddListener(onGrabbingBall);
    }



    #region INPUT EVENT CALLBACKS

    public void onPunch(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            Debug.Log("Pressing Punch Button");
            if (!isBusy)
            {
                StartCoroutine(punch());
            }
        }
        else if(context.canceled)
        {
            Debug.Log("Releasing Punch Button");
            if (isWindingUpPunch)
            {
                StartCoroutine(taunt());
            }
        }
    }

 
    public void onFortify(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            if (!isBusy)
            {
                StartCoroutine(fortify());
            }
        }
    }

    #endregion


    #region GAMEPLAY EVENTS CALLBACKS

    // void onHitBySlide(Player source)
    // {
    //     if (isPunching != true && !isInvincible)
    //     {
    //         StartCoroutine(slideReactWindow(source));
    //         return;
    //     }
    //     slideCounter(source);
    // }
    
    void onHitByPunch(Player source)
    {
        if (isFortified != true && !isInvincible)
        {
            StartCoroutine(punchReactWindow(source));
            return;
        }
        punchCounter(source);
    }
    
    void onHitByBallPunch(Player source)
    {
        if (isFortified != true && !isInvincible)
        {
            StartCoroutine(ballPunchReactWindow(source));
            return;
        }
        ballPunchCounter(source);
    }
    
    // void onHitByBallSlide(Player source)
    // {
    //     if (isPunching != true && !isInvincible)
    //     {
    //         StartCoroutine(ballSlideReactWindow(source));
    //         return;
    //     }
    //     ballSlideCounter(source);
    // }

    void onGrabbingBall()
    {
        if (isStunned)
        {
            return;
        }
        isHoldingBall = true;
        GameManager.Instance.ball.Toggle(false);
        GameManager.Instance.ball.transform.SetParent(ballAnchorPoint);
        GameManager.Instance.ball.transform.position = ballAnchorPoint.position;
        player.data.grabbingBallSound.Post(gameObject);
        GameManager.Instance.OnBallGrabbed(gameObject.GetComponent<Player>());
    }

    public void onDeath(Player source)
    {
        StartCoroutine(death(source));
    }

    #endregion


    #region COMBAT COROUTINES

    
    IEnumerator punch()
    {
        ColliderBox colliderBox;
        if(player.controller.steerInput < 0)
        {
            isWindingUpPunchLeft = true;
            player.anime.SetTrigger("WindUp.L");
            colliderBox = punchLeftCollider;
        }
        else if (player.controller.steerInput > 0)
        {
            isWindingUpPunchRight = true;
            player.anime.SetTrigger("WindUp.R");
            colliderBox = punchRightCollider;
        }
        else
            yield break;
        
        yield return new WaitForSeconds(player.data.punchWindUp);
        
        if (isTaunting)
        {
            yield break;
        }

        if (isHoldingBall)
        {
            player.data.balLPunchSound.Post(gameObject);
            colliderBox.SetType(ColliderBox.ColliderType.BallPunch);
        } else
        {
            player.data.punchSound.Post(gameObject);
            colliderBox.SetType(ColliderBox.ColliderType.Punch);
        }

        isPunchingLeft = isWindingUpPunchLeft;
        isPunchingRight = isWindingUpPunchRight;
        isWindingUpPunchLeft = false;
        isWindingUpPunchRight = false;
        colliderBox.Toggle(true);

        yield return new WaitForSeconds(player.data.punchDamageWindow);
        
        colliderBox.Toggle(false);
        isPunchingLeft = false;
        isPunchingRight = false;
        isRecovering = true;
        
        yield return new WaitForSeconds(player.data.actionsCooldown);
        
        isRecovering = false;
    }

    IEnumerator fortify()
    {
        isFortified = true;
        player.anime.SetTrigger("Parry");
        yield return new WaitForSeconds(player.data.parryWindow);
        isFortified = false;
        isRecovering = true;
        yield return new WaitForSeconds(player.data.actionsCooldown);
        isRecovering = false;
    }
    
    IEnumerator taunt()
    {
        isWindingUpPunchLeft = false;
        isWindingUpPunchRight = false;
        isTaunting = true;
        player.anime.SetTrigger("Taunt");
        yield return new WaitForSeconds(0.5f);
        player.anime.ResetTrigger("Taunt");
        isTaunting = false;
        isRecovering = true;
        yield return new WaitForSeconds(player.data.actionsCooldown);
        isRecovering = false;
    }

    IEnumerator punchReactWindow(Player source)
    {
        yield return new WaitForSeconds(player.data.counterWindow);
        if (isFortified != true)
        {
            punchHit(source);
        }
        else
        {
            punchCounter(source);
        }
    }
    
    IEnumerator ballPunchReactWindow(Player source)
    {
        yield return new WaitForSeconds(player.data.counterWindow);
        if (isFortified != true)
        {
            ballPunchHit(source);
        }
        else
        {
            ballPunchCounter(source);
        }
    }

    IEnumerator stun(Player source)
    {
        if (isHoldingBall)
        {
            isHoldingBall = false;
            if (source != null)
            {
                source.combat.onGrabbingBall();
            }
            else
            {
                dropBall(player.character.transform);
            }
        }
        isStunned = true;
        player.controller.rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(player.data.stunDuration);
        isStunned = false;
        StartCoroutine(invincibility());
    }
    
    IEnumerator death(Player source)
    {
        if (isHoldingBall)
        {
            isHoldingBall = false;
            if (source != null)
            {
                source.combat.onGrabbingBall();
            }
            else
            {
                dropBall(player.character.transform);
            }
        }
        if(source == GameManager.Instance.ballHolder)
            GameManager.Instance.OnBallKill();
        isStunned = true;
        player.controller.rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(player.data.stunDuration);
        isStunned = false;
        GameManager.Instance.OnPlayerDeath(player);
        StartCoroutine(invincibility());
    }
    
    IEnumerator invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(player.data.invincibilityDuration);
        isInvincible = false;
    }
    
    #endregion
    
    
    #region COMBAT HITS & COUNTERS

    void dropBall(Transform pos)
    {
        GameManager.Instance.ball.Toggle(true);
        GameManager.Instance.ball.transform.parent = null;
        GameManager.Instance.ball.transform.position = pos.position;
        GameManager.Instance.ball.transform.rotation = pos.rotation;
    }

    void punchHit(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.green;
        player.data.punchHitSound.Post(gameObject);
        particleSystem.Play();
        StartCoroutine(stun(source));
    }

    void punchCounter(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.white;
        player.data.punchCounterSound.Post(gameObject);
        particleSystem.Play();
    }

    
    void ballPunchHit(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.red;
        player.data.ballPunchHitSound.Post(gameObject);
        particleSystem.Play();
        StartCoroutine(death(source));
    }

    void ballPunchCounter(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.magenta;
        player.data.ballPunchCounterSound.Post(gameObject);
        particleSystem.Play(source);
    }
    #endregion

}
