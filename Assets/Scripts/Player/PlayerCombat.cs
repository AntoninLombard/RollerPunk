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
    public bool isBusy => isPunchingLeft || isPunchingRight || isRecovering || isFortified || isStunned || isTaunting || isWindingUpPunchLeft || isWindingUpPunchRight || player.controller.isDrifting;
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
    [SerializeField] private ColliderBox punchLeftCollider;
    [SerializeField] private ColliderBox punchRightCollider;


    private Gamepad gamepad;
    
    
    
    private static readonly int ParrySuccess = Animator.StringToHash("ParrySuccess");
    private static readonly int Parry = Animator.StringToHash("Parry");
    private static readonly int Taunt = Animator.StringToHash("Taunt");
    private static readonly int Stunned = Animator.StringToHash("Stunned");
    private static readonly int GetUp = Animator.StringToHash("GetUp");
    private static readonly int Countered = Animator.StringToHash("Countered");
    private static readonly int WindUpL = Animator.StringToHash("WindUp.L");
    private static readonly int WindUpR = Animator.StringToHash("WindUp.R");
    private static readonly int Punch = Animator.StringToHash("Punch");
    private static readonly int BallGrab = Animator.StringToHash("BallGrab");


    private void Awake()
    {
        //gamepad = (Gamepad)player.input.devices.Where(x => x.GetType() == gamepad.GetType() );
        OnHitByPunch.AddListener(onHitByPunch);
        OnGrabbingBall.AddListener(grabbingBall);
    }

    


    #region INPUT EVENT CALLBACKS

    public void StartPunch()
    {
        if (!isBusy)
        {
            StartCoroutine(punch());
            player.data.windUpSound.Post(gameObject);
        }
    }
    
    public void CancelPunch()
    {
        if (isWindingUpPunch)
        {
            player.anime.CancelPunchWindUp();
            StartCoroutine(taunt());
        } else if (!(isTaunting || isRecovering || !isHoldingPunch || isStunned))
        {
            player.anime.CancelPunchWindUp();
            StartCoroutine(punchRelease());
        }
    }

    public void StartParry()
    {
        if (!isBusy)
        {
            StartCoroutine(parry());
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
    

    void grabbingBall()
    {
        if (isStunned)
        {
            return;
        }
        isHoldingBall = true;
        GameManager.Instance.ball.Toggle(false);
        GameManager.Instance.ball.SetEmissiveColor(player.color);
        GameManager.Instance.ball.AttachTo(ballAnchorPoint);
        player.data.grabbingBallSound.Post(gameObject);
        GameManager.Instance.gameData.musicState[player.number].SetValue();
        GameManager.Instance.BallGrabbed(gameObject.GetComponent<Player>());
        if (!player.combat.isBusy)
        {
            player.anime.animator.SetTrigger(BallGrab);
        }
    }
    

    public void Kill(Player source)
    {
        StartCoroutine(death(source));
    }

    public void onParryHit(Player source)
    {
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
        int punchSide = player.input.lastSteerSide;
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
        player.anime.StartPunchWindUp(punchSide);
        
        yield return new WaitForSeconds(player.data.punchWindUp);
        if (isTaunting || isRecovering || !isWindingUpPunch || isStunned)
        {
            player.anime.CancelPunchWindUp();
            yield break;
        }
        isHoldingPunchLeft = punchSide < 0;
        isHoldingPunchRight = punchSide > 0;
        isWindingUpPunchLeft = false;
        isWindingUpPunchRight = false;
        
        
        
        yield return new WaitForSeconds(player.data.punchHoldDuration);
        if (isTaunting || isRecovering || !isHoldingPunch || isStunned || isPunching)
        {
            player.anime.CancelPunchWindUp();
            yield break;
        } 
        yield return punchRelease();
    }
    

    IEnumerator punchRelease()
    {
        player.anime.animator.SetTrigger(Punch);
        
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
        collider.Toggle(true);
        
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
        if (player.controller.isDrifting)
        {
            player.controller.CancelDrift(false);
        }
        if (isHoldingBall)
        {
            isHoldingBall = false;
            if (source != null)
            {
                source.combat.grabbingBall();

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
        player.data.gettingUpSound.Post(gameObject);
        isStunned = false;
        isInvincible = false;
    }
    
    IEnumerator death(Player source)
    {
        if (player.controller.isDrifting)
        {
            player.controller.CancelDrift(false);
        }
        if (isHoldingBall)
        {
            isHoldingBall = false;
            if (source != null)
            {
                GameManager.Instance.gameData.crowdSteal.Post(gameObject);
                source.combat.grabbingBall();
            }
            else
            {
                dropBall(player.character.transform);
            }
        }
        if(source != null && source == GameManager.Instance.ballHolder)
            GameManager.Instance.BallKill(player);
        isStunned = true;
        isInvincible = true;
        player.controller.rb.velocity = Vector3.zero;
        player.anime.Death();
        yield return new WaitForSeconds(player.data.stunDuration);
        isStunned = false;
        isInvincible = false;
        player.anime.Respawn();
        GameManager.Instance.RespawnPlayer(player);
        player.data.respawnSound.Post(gameObject);
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
        GameManager.Instance.BallDropped();
    }

    void punchHit(Player source)
    {
        if(source.combat.isHoldingBall)
        {
            player.data.ballPunchHitSound.Post(gameObject);
            GameManager.Instance.gameData.crowdStun.Post(GameManager.Instance.gameObject);
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
        player.data.punchCounterSound.Post(gameObject);
        player.anime.animator.SetTrigger(Countered);
        //StartCoroutine(stun(source));
    }
    
    void parry(Player source)
    {
        player.anime.animator.SetTrigger(ParrySuccess);
        source.combat.onParryHit(player);
    }
    #endregion

    public void Fall()
    {
        if (isHoldingBall)
        {
            GameManager.Instance.gameData.crowdFall.Post(GameManager.Instance.gameObject);
            GameManager.Instance.gameData.musicState[4].SetValue();
        }
        StartCoroutine(death(null));
        player.data.startEngineSound.Post(gameObject);
        
    }

    // int sourceDirection(Transform source)
    // {
    //     Vector3 dir = source.position - player.character.position;
    //     float value = Vector3.Dot(player.character.right, dir);
    //     
    //     return value > 0 ?  1 : -1;
    // }


    public void ToggleInvincibility(bool isInvincible)
    {
        this.isInvincible = isInvincible;
    }
}
