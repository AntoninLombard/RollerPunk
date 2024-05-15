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
    [field: SerializeField] public bool isPunchingLeft { get; private set; }
    [field: SerializeField] public bool isPunchingRight { get; private set; }
    [field: SerializeField] public bool isFortified { get; private set; }
    [field: SerializeField] public bool isSliding { get; private set; }
    [field: SerializeField] public bool isStunned { get; private set; }
    [field: SerializeField] public bool isHoldingBall { get; private set; }

    
    [Header("Events")] 
    [SerializeField] public UnityEvent<Player> OnHitByPunch;
    //[SerializeField] public UnityEvent<Player> OnHitbySlide;
    [SerializeField] public UnityEvent<Player> OnHitbByBallPunch;
    //[SerializeField] public UnityEvent<Player> OnHitbByBallSlide;
    [SerializeField] public UnityEvent OnGrabbingBall;
    
    
    [Header("Parts")] 
    [SerializeField] private Transform ballAnchorPoint;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private ColliderBox punchLeftCollider;
    [SerializeField] private ColliderBox punchRightCollider;
    
    
    private void Awake()
    {
        OnHitByPunch.AddListener(onHitByPunch);
        //OnHitbySlide.AddListener(onHitBySlide);
        OnHitbByBallPunch.AddListener(onHitByBallPunch);
        //OnHitbByBallSlide.AddListener(onHitByBallSlide);
        OnGrabbingBall.AddListener(onGrabbingBall);
    }



    #region INPUT EVENT CALLBACKS

    public void onPunchLeft(InputAction.CallbackContext context)
    {
        if (!isSliding && !isPunchingLeft & !isPunchingRight && player.controller.isGrounded && !isRecovering)
        {
            if (!isHoldingBall)
            {
                StartCoroutine(punchLeft());
            }
            else
            {
                StartCoroutine(ballPunchLeft());
            }
        }
    }

    public void onPunchRight(InputAction.CallbackContext context)
    {
        if (!isSliding && !isPunchingLeft & !isPunchingRight && player.controller.isGrounded && !isRecovering && !isFortified)
        {
            if (!isHoldingBall)
            {
                StartCoroutine(punchRight());
            }
            else
            {
                StartCoroutine(ballPunchRight());
            }

        }
    }

    public void onFortify(InputAction.CallbackContext context)
    {
        if (!isSliding && !isPunchingLeft & !isPunchingRight && player.controller.isGrounded && !isRecovering && !isFortified)
        {
            StartCoroutine(fortify());
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

    IEnumerator punchLeft()
    {
        isPunchingLeft = true;
        player.data.punchSound.Post(gameObject);
        player.anime.SetBool("WindUp.L",true);
        punchLeftCollider.Toggle(true);
        punchLeftCollider.SetType(ColliderBox.ColliderType.Punch);
        yield return new WaitForSeconds(0.5f);
        punchLeftCollider.Toggle(false);
        player.anime.SetBool("WindUp.L",false);
        isPunchingLeft = false;
        isRecovering = true;
        yield return new WaitForSeconds(player.data.actionsCooldown);
        isRecovering = false;
    }
    
    IEnumerator punchRight()
    {
        isPunchingRight = true;
        player.data.punchSound.Post(gameObject);
        player.anime.SetBool("WindUp.R",true);
        punchRightCollider.Toggle(true);
        punchRightCollider.SetType(ColliderBox.ColliderType.Punch);
        yield return new WaitForSeconds(0.5f);
        punchRightCollider.Toggle(false);
        player.anime.SetBool("WindUp.R",false);
        isPunchingRight = false;
        isRecovering = true;
        yield return new WaitForSeconds(player.data.actionsCooldown);
        isRecovering = false;
    }

    IEnumerator fortify()
    {
        isFortified = true;
        player.anime.SetTrigger("Parry");
        yield return new WaitForSeconds(0.5f);
        isFortified = false;
        yield return new WaitForSeconds(player.data.actionsCooldown);
    }
    
    // IEnumerator slide()
    // {
    //     isSliding = true;
    //     player.data.slideSound.Post(gameObject);
    //     slideCollider.Toggle(true);
    //     punchCollider.SetType(ColliderBox.ColliderType.Slide);
    //     yield return new WaitForSeconds(0.5f);
    //     slideCollider.Toggle(false);
    //     isSliding = false;
    //     isRecovering = true;
    //     yield return new WaitForSeconds(player.data.actionsCooldown);
    //     isRecovering = false;
    // }
    
    
    IEnumerator ballPunchLeft()
    {
        isPunchingLeft = true;
        player.data.balLPunchSound.Post(gameObject);
        punchLeftCollider.Toggle(true);
        player.anime.SetBool("WindUp.L",true);
        punchLeftCollider.SetType(ColliderBox.ColliderType.BallPunch);
        yield return new WaitForSeconds(0.5f);
        punchLeftCollider.Toggle(false);
        player.anime.SetBool("WindUp.L",false);
        isPunchingLeft = false;
        isRecovering = true;
        yield return new WaitForSeconds(player.data.actionsCooldown);
        isRecovering = false;
    }
    
    IEnumerator ballPunchRight()
    {
        isPunchingRight = true;
        player.data.balLPunchSound.Post(gameObject);
        punchRightCollider.Toggle(true);
        player.anime.SetBool("WindUp.R",true);
        punchRightCollider.SetType(ColliderBox.ColliderType.BallPunch);
        yield return new WaitForSeconds(0.5f);
        punchRightCollider.Toggle(false);
        player.anime.SetBool("WindUp.R",false);
        isPunchingRight = false;
        isRecovering = true;
        yield return new WaitForSeconds(player.data.actionsCooldown);
        isRecovering = false;
    }
    
    // IEnumerator ballSlide()
    // {
    //     isSliding = true;
    //     player.data.ballSlideSound.Post(gameObject);
    //     slideCollider.Toggle(true);
    //     slideCollider.SetType(ColliderBox.ColliderType.BallSlide);
    //     yield return new WaitForSeconds(0.5f);
    //     slideCollider.Toggle(true);
    //     isSliding = false;
    //     isRecovering = true;
    //     yield return new WaitForSeconds(player.data.actionsCooldown);
    //     isRecovering = false;
    // }

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
    
    // IEnumerator slideReactWindow(Player source)
    // {
    //     yield return new WaitForSeconds(player.data.counterWindow);
    //     if (isSliding != true)
    //     {
    //         slideHit(source);
    //     }
    //     else
    //     {
    //         slideCounter(source);
    //     }
    // }
    
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
    
    // IEnumerator ballSlideReactWindow(Player source)
    // {
    //     yield return new WaitForSeconds(player.data.counterWindow);
    //     if (isPunching != true)
    //     {
    //         ballSlideHit(source);
    //     }
    //     else
    //     {
    //         ballSlideCounter(source);
    //     }
    // }
    
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

    void slideHit(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.yellow;
        player.data.slideHitSound.Post(gameObject);
        particleSystem.Play();
        StartCoroutine(stun(source));
    }

    void slideCounter(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.white;
        player.data.slideCounterSound.Post(gameObject);
        particleSystem.Play();
    }

    void ballSlideHit(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.red;
        player.data.ballSlideHitSound.Post(gameObject);
        particleSystem.Play();
        StartCoroutine(death(source));
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
    
    void ballSlideCounter(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.magenta;
        player.data.ballSlideCounterSound.Post(gameObject);
        particleSystem.Play();
    }

    #endregion

}
