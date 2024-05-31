using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.VFX;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Player player;
    
    [Header("Player Inputs")] 
    [SerializeField] private bool punchInput;
    [SerializeField] private bool slideInput;
    [SerializeField] private bool fortifyInput;


    [field: Header("CHARACTER STATE")]
    [SerializeField] public int lastSteerSide;
    [field: SerializeField] public bool isInvincible { get; private set; }
    [field: SerializeField] public bool isRecovering { get; private set; }
    [field: SerializeField] public bool isWindingUpPunchLeft { get; private set; }
    [field: SerializeField] public bool isWindingUpPunchRight { get; private set; }
    [field: SerializeField] public bool isHoldingPunchLeft { get; private set; }
    [field: SerializeField] public bool isHoldingPunchRight { get; private set; }
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
    public bool isHoldingPunch => isHoldingPunchLeft || isHoldingPunchRight;

    [Header("Events")] 
    [SerializeField] public UnityEvent<Player> OnHitByPunch;
    [SerializeField] public UnityEvent OnGrabbingBall;
    [SerializeField] public UnityEvent OnHitByParry;
    [SerializeField] public UnityEvent OnHitByCounter;
    
    
    
    [Header("Parts")] 
    [SerializeField] private Transform ballAnchorPoint;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private ColliderBox punchLeftCollider;
    [SerializeField] private ColliderBox punchRightCollider;


    private Gamepad gamepad;
    
    
    
    private static readonly int ParrySuccessR = Animator.StringToHash("ParrySuccess.R");
    private static readonly int ParrySuccessL = Animator.StringToHash("ParrySuccess.L");
    private static readonly int Parry = Animator.StringToHash("Parry");
    private static readonly int Taunt = Animator.StringToHash("Taunt");
    private static readonly int Stunned = Animator.StringToHash("Stunned");
    private static readonly int GetUp = Animator.StringToHash("GetUp");
    private static readonly int Countered = Animator.StringToHash("Countered");
    private static readonly int WindUpL = Animator.StringToHash("WindUp.L");
    private static readonly int WindUpR = Animator.StringToHash("WindUp.R");


    private void Awake()
    {
        //gamepad = (Gamepad)player.input.devices.Where(x => x.GetType() == gamepad.GetType() );
        OnHitByPunch.AddListener(onHitByPunch);
        OnGrabbingBall.AddListener(onGrabbingBall);
    }

    private void Update()
    {
        if(player.controller?.steerInput != 0)
            lastSteerSide = (player.controller?.steerInput > 0)? 1 : -1;
    }
    


    #region INPUT EVENT CALLBACKS

    public void onPunch(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            punchInput = true;
            Debug.Log("Pressing Punch Button");
            if (!isBusy)
            {
                StartCoroutine(punch());
            }
        }
        else if(context.canceled)
        {
            punchInput = false;
            Debug.Log("Releasing Punch Button");
            if (isWindingUpPunch)
            {
                StartCoroutine(taunt());
            } else if (!(isTaunting || isRecovering || !isHoldingPunch || isStunned))
            {
                StartCoroutine(punchRelease());
            }
        }
    }

 
    public void onParry(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            fortifyInput = true;
            if (!isBusy)
            {
                StartCoroutine(parry());
            }
        } else if (context.canceled)
        {
            fortifyInput = true;
        }
    }
    
    public void onReady(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            player.isReady = true;
            player.ui.SetReady(true);
            GameManager.Instance.OnPlayerReady();
            Debug.Log("StartPressed");
        } 

    }

    #endregion


    #region GAMEPLAY EVENTS CALLBACKS
    
    
    void onHitByPunch(Player source)
    {
        if (isInvincible)
        {
            return;
        }

        if (isFortified)
        {
            parry(source);
            return;
        }
        
        if (isPunching)
        {
            counter(source);
            if (isHoldingBall)
                player.data.ballPunchCounterSound.Post(gameObject);
            else
                player.data.punchCounterSound.Post(gameObject);
        }
        else
        {
            StartCoroutine(punchReactWindow(source));
        }
    }
    

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

    public void onParryHit(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.red;
        particleSystem.Play();
        if(source.combat.isHoldingBall)
        {
            StartCoroutine(death(source));
        }
        else
        {
            StartCoroutine(stun(source));
        }
        player.data.parrySuccessSound.Post(gameObject);
    }

    #endregion


    #region COMBAT COROUTINES

    
    IEnumerator punch()
    {
        ColliderBox colliderBox;
        int punchSide = lastSteerSide;
        switch (punchSide)
        {
            case < 0:
                isWindingUpPunchLeft = true;
                player.anime.animator.SetTrigger(WindUpL);
                colliderBox = punchLeftCollider;
                break;
            case > 0:
                isWindingUpPunchRight = true;
                player.anime.animator.SetTrigger(WindUpR);
                colliderBox = punchRightCollider;
                break;
            default:
                yield break;
        }
        
        yield return new WaitForSeconds(player.data.punchWindUp);
        if (isTaunting || isRecovering || !isWindingUpPunch || isStunned)
        {
            yield break;
        }
        //TODO start animation here.
        isHoldingPunchLeft = punchSide < 0;
        isHoldingPunchRight = punchSide > 0;
        isWindingUpPunchLeft = false;
        isWindingUpPunchRight = false;
        
        
        
        yield return new WaitForSeconds(player.data.punchHoldDuration);
        if (isTaunting || isRecovering || !isHoldingPunch || isStunned || isPunching)
        {
            yield break;
        } 
        yield return punchRelease();
    }
    

    IEnumerator punchRelease()
    {
        //TODO Trigger punch here
        
        isPunchingLeft = isHoldingPunchLeft;
        isPunchingRight = isHoldingPunchRight;
        isHoldingPunchLeft = false;
        isHoldingPunchRight = false;

        ColliderBox collider = isPunchingLeft ? punchLeftCollider : punchRightCollider;
        
        if (isHoldingBall)
        {
            player.data.balLPunchSound.Post(gameObject);
        } else
        {
            player.data.punchSound.Post(gameObject);
        }
        
        yield return new WaitForSeconds(player.data.punchDamageWindow);
        
        collider.Toggle(false);
        isPunchingLeft = false;
        isPunchingRight = false;
        yield return cooldown(player.data.punchCooldown);
    }

    IEnumerator cooldown(float duration)
    {
        isRecovering = true;
        
        yield return new WaitForSeconds(duration);
        
        isRecovering = false;
    }

    IEnumerator parry()
    {
        isFortified = true;
        player.anime.animator.SetTrigger(Parry);
        player.data.parrySound.Post(gameObject);
        yield return new WaitForSeconds(player.data.parryWindow);
        player.anime.animator.ResetTrigger(Parry);
        isFortified = false;
        isRecovering = true;
        yield return new WaitForSeconds(player.data.parryCooldown);
        isRecovering = false;
    }
    
    IEnumerator taunt()
    {
        isWindingUpPunchLeft = false;
        isWindingUpPunchRight = false;
        isTaunting = true;
        StopCoroutine(punch());
        player.anime.animator.SetTrigger(Taunt);
        player.data.punchTauntSound.Post(gameObject);
        yield return new WaitForSeconds(player.data.tauntDuration);
        player.anime.animator.ResetTrigger(Taunt);
        isTaunting = false;
        // isRecovering = true;
        // yield return new WaitForSeconds(player.data.actionsCooldown);
        // isRecovering = false;
    }

    IEnumerator punchReactWindow(Player source)
    {
        yield return new WaitForSeconds(player.data.counterWindow);
        if (isFortified)
        {
            parry(source);
        }
        else if(isPunching)
        {
            counter(source);
        }
        else
        {
            punchHit(source);
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
        isInvincible = true;
        player.controller.rb.velocity = Vector3.zero;
        player.anime.animator.SetTrigger(Stunned);
        yield return new WaitForSeconds(player.data.stunDuration);
        player.anime.animator.SetTrigger(GetUp);
        isStunned = false;
        isInvincible = false;
    }
    
    IEnumerator death(Player source)
    {
        if (isHoldingBall)
        {
            isHoldingBall = false;
            if (source != null)
            {
                GameManager.Instance.gameData.crowdSteal.Post(gameObject);
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
        isInvincible = true;
        player.controller.rb.velocity = Vector3.zero;
        player.anime.animator.SetTrigger(Stunned);
        yield return new WaitForSeconds(player.data.stunDuration);
        player.anime.animator.SetTrigger(GetUp);
        player.data.gettingUpSound.Post(gameObject);
        isStunned = false;
        isInvincible = false;
        GameManager.Instance.RespawnPlayer(player);
        yield return invincibility();
    }
    
    IEnumerator invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(player.data.invincibilityDuration);
        isInvincible = false;
    }
    
    #endregion
    
    
    #region COMBAT HITS & COUNTERS
    
    
    public void dropBall(Transform pos)
    {
        isHoldingBall = false;
        GameManager.Instance.ball.Toggle(true);
        GameManager.Instance.ball.transform.parent = null;
    }

    void punchHit(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.red;
        particleSystem.Play();
        if(source.combat.isHoldingBall)
        {
            player.data.ballPunchHitSound.Post(gameObject);
            StartCoroutine(death(source));
        }
        else
        {
            player.data.punchHitSound.Post(gameObject);
            StartCoroutine(stun(source));
        }
    }
    
    
    void counter(Player source)
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.white;
        particleSystem.Play();
        player.data.punchCounterSound.Post(gameObject);
        player.anime.animator.SetTrigger(Countered);
        //StartCoroutine(stun(source));
    }
    
    void parry(Player source)
    {
        int  dir = sourceDirection(source.character);
        if (dir > 0) // RIGHT
        {
            player.anime.animator.SetTrigger(ParrySuccessR);
        }
        else if(dir < 0) // LEFT
        {
            player.anime.animator.SetTrigger(ParrySuccessL);
        }
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.green;
        particleSystem.Play();
        source.combat.onParryHit(player);
    }
    #endregion

    public void onFall()
    {
        StartCoroutine(stun(null));
        GameManager.Instance.RespawnPlayer(player);
        player.data.respawnSound.Post(gameObject);
    }

    int sourceDirection(Transform source)
    {
        Vector3 dir = source.position - player.character.position;
        float value = Vector3.Dot(player.character.right, dir);
        
        return value > 0 ?  1 : -1;
    }


    public void ToggleInvincibility(bool isInvincible)
    {
        this.isInvincible = isInvincible;
    }
}
