using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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
    [field: SerializeField] public bool isPunching { get; private set; }
    [field: SerializeField] public bool isSliding { get; private set; }
    [field: SerializeField] public bool isStunned { get; private set; }
    [field: SerializeField] public bool isHoldingBall { get; private set; }

    
    [Header("Events")] 
    [SerializeField] public UnityEvent<Player> OnHitByPunch;
    [SerializeField] public UnityEvent<Player> OnHitbySlide;
    [SerializeField] public UnityEvent<Player> OnHitbByBallPunch;
    [SerializeField] public UnityEvent<Player> OnHitbByBallSlide;
    [SerializeField] public UnityEvent<GameObject> OnGrabbingBall;
    
    
    [Header("Parts")] 
    [SerializeField] private Transform ballAnchorPoint;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private ColliderBox punchCollider;
    [SerializeField] private ColliderBox slideCollider;

    
    private void Awake()
    {
        OnHitByPunch.AddListener(onHitByPunch);
        OnHitbySlide.AddListener(onHitBySlide);
        OnHitbByBallPunch.AddListener(onHitByBallPunch);
        OnHitbByBallSlide.AddListener(onHitByBallSlide);
        OnGrabbingBall.AddListener(onGrabbingBall);
    }
    
     
    #region INPUT EVENT CALLBACKS

    public void onPunch(InputAction.CallbackContext context)
    {
        if (!isSliding && !isPunching && player.controller.isGrounded && !isRecovering)
        {
            if (!isHoldingBall)
            {
                StartCoroutine(punch());
            }
            else
            {
                StartCoroutine(ballPunch());
            }
        }
    }

    public void onSlide(InputAction.CallbackContext context)
    {
        if (!isSliding && !isPunching && player.controller.isGrounded && !isRecovering)
        {
            if (!isHoldingBall)
            {
                StartCoroutine(slide());
            }
            else
            {
                StartCoroutine(ballSlide());
            }

        }
    }

    public void onFortify(InputAction.CallbackContext context)
    {
        //fortifyInput = !fortifyInput;
    }

    #endregion


    #region GAMEPLAY EVENTS CALLBACKS

    void onHitBySlide(Player source)
    {
        if (isPunching != true && !isInvincible)
        {
            StartCoroutine(slideReactWindow(source));
            return;
        }
        slideCounter(source);
    }
    
    void onHitByPunch(Player source)
    {
        if (isPunching != true && !isInvincible)
        {
            StartCoroutine(punchReactWindow(source));
            return;
        }
        punchCounter(source);
    }
    
    void onHitByBallPunch(Player source)
    {
        if (isPunching != true && !isInvincible)
        {
            StartCoroutine(ballPunchReactWindow(source));
            return;
        }
        ballPunchCounter(source);
    }
    
    void onHitByBallSlide(Player source)
    {
        if (isPunching != true && !isInvincible)
        {
            StartCoroutine(ballSlideReactWindow(source));
            return;
        }
        ballSlideCounter(source);
    }

    void onGrabbingBall(GameObject ball)
    {
        isHoldingBall = true;
        ball.transform.SetParent(ballAnchorPoint);
        ball.transform.position = ballAnchorPoint.position;
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
        isPunching = true;
        player.data.punchSound.Post(gameObject);
        punchCollider.Toggle(true);
        punchCollider.SetType(ColliderBox.ColliderType.Punch);
        yield return new WaitForSeconds(0.5f);
        punchCollider.Toggle(false);
        isPunching = false;
        isRecovering = true;
        yield return new WaitForSeconds(player.data.actionsCooldown);
        isRecovering = false;
    }
    
    IEnumerator slide()
    {
        isSliding = true;
        player.data.slideSound.Post(gameObject);
        slideCollider.Toggle(true);
        punchCollider.SetType(ColliderBox.ColliderType.Slide);
        yield return new WaitForSeconds(0.5f);
        slideCollider.Toggle(false);
        isSliding = false;
        isRecovering = true;
        yield return new WaitForSeconds(player.data.actionsCooldown);
        isRecovering = false;
    }
    
    
    IEnumerator ballPunch()
    {
        isPunching = true;
        player.data.balLPunchSound.Post(gameObject);
        punchCollider.Toggle(true);
        punchCollider.SetType(ColliderBox.ColliderType.BallPunch);
        yield return new WaitForSeconds(0.5f);
        punchCollider.Toggle(false);
        isPunching = false;
        isRecovering = true;
        yield return new WaitForSeconds(player.data.actionsCooldown);
        isRecovering = false;
    }
    
    IEnumerator ballSlide()
    {
        isSliding = true;
        player.data.ballSlideSound.Post(gameObject);
        slideCollider.Toggle(true);
        slideCollider.SetType(ColliderBox.ColliderType.BallSlide);
        yield return new WaitForSeconds(0.5f);
        slideCollider.Toggle(true);
        isSliding = false;
        isRecovering = true;
        yield return new WaitForSeconds(player.data.actionsCooldown);
        isRecovering = false;
    }

    IEnumerator punchReactWindow(Player source)
    {
        yield return new WaitForSeconds(player.data.counterWindow);
        if (isPunching != true)
        {
            punchHit(source);
        }
        else
        {
            punchCounter(source);
        }
    }
    
    IEnumerator slideReactWindow(Player source)
    {
        yield return new WaitForSeconds(player.data.counterWindow);
        if (isSliding != true)
        {
            slideHit(source);
        }
        else
        {
            slideCounter(source);
        }
    }
    
    IEnumerator ballPunchReactWindow(Player source)
    {
        yield return new WaitForSeconds(player.data.counterWindow);
        if (isPunching != true)
        {
            ballPunchHit(source);
        }
        else
        {
            ballPunchCounter(source);
        }
    }
    
    IEnumerator ballSlideReactWindow(Player source)
    {
        yield return new WaitForSeconds(player.data.counterWindow);
        if (isPunching != true)
        {
            ballSlideHit(source);
        }
        else
        {
            ballSlideCounter(source);
        }
    }
    
    IEnumerator stun(Player source)
    {
        isStunned = true;
        player.controller.rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(player.data.stunDuration);
        isStunned = false;
        StartCoroutine(invincibility());
    }
    
    IEnumerator death(Player source)
    {
        if(!(source == null))
            GameManager.Instance.OnScoreChange(source,1);
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

    void punchHit(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = GetComponent<ParticleSystem>().main;
        particleSystemMain.startColor = Color.green;
        player.data.punchHitSound.Post(gameObject);
        GetComponent<ParticleSystem>().Play();
        StartCoroutine(stun(source));
    }

    void punchCounter(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = GetComponent<ParticleSystem>().main;
        particleSystemMain.startColor = Color.white;
        player.data.punchCounterSound.Post(gameObject);
        GetComponent<ParticleSystem>().Play();
    }

    void slideHit(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = GetComponent<ParticleSystem>().main;
        particleSystemMain.startColor = Color.yellow;
        player.data.slideHitSound.Post(gameObject);
        GetComponent<ParticleSystem>().Play();
        StartCoroutine(stun(source));
    }

    void slideCounter(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = GetComponent<ParticleSystem>().main;
        particleSystemMain.startColor = Color.white;
        player.data.slideCounterSound.Post(gameObject);
        GetComponent<ParticleSystem>().Play();
    }

    void ballSlideHit(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = GetComponent<ParticleSystem>().main;
        particleSystemMain.startColor = Color.red;
        player.data.ballSlideHitSound.Post(gameObject);
        GetComponent<ParticleSystem>().Play();
        StartCoroutine(death(source));
    }
    
    void ballPunchHit(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = GetComponent<ParticleSystem>().main;
        particleSystemMain.startColor = Color.red;
        player.data.ballPunchHitSound.Post(gameObject);
        GetComponent<ParticleSystem>().Play();
        StartCoroutine(death(source));
    }

    void ballPunchCounter(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = GetComponent<ParticleSystem>().main;
        particleSystemMain.startColor = Color.magenta;
        player.data.ballPunchCounterSound.Post(gameObject);
        GetComponent<ParticleSystem>().Play(source);
    }
    
    void ballSlideCounter(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = GetComponent<ParticleSystem>().main;
        particleSystemMain.startColor = Color.magenta;
        player.data.ballSlideCounterSound.Post(gameObject);
        GetComponent<ParticleSystem>().Play();
    }

    #endregion

}
