using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerControllerData : ScriptableObject
{
    [Header("DRIVING VALUES")]
    public float maxSpeed;
    public float forwardAccel = 0;
    public float turningRate = 0;
    public float gravityStrength = 0.0f;
    public float friction;
    public float gripAccel = 0.0f;
    public float boostAccel = 0.0f;
    public float boostDuration = 0.0f;
    public float driftAccel;
    public float driftSteerOffset;
    public float driftTurnMultiplier;
    public float driftDurationForBoost;
    [Range(0.0f,1.0f)] public float brakingRatio = 0;
    [Range(0.0f,1.0f)] public float drag;
    [Range(0.0f,1.0f)] public float airDrag;
    [Range(0.2f,1.0f)] public float ballAccelMultiplier;
    [Range(0.2f,1.0f)] public float ballMaxSpeedMultipier;

    [Header("COMBAT VALUES")]
    public float punchWindUp;
    public float punchDamageWindow;
    public float tauntDuration;
    public float punchHoldDuration;
    public float parryWindow;
    public float counterWindow;
    public float punchCooldown;
    public float parryCooldown;
    public float stunDuration;
    public float invincibilityDuration;

    [Header("CAMERA EFFECTS")] 
    public float minFOV;
    public float maxFOV;
    
    
    [Header("SOUND")] 
    public AK.Wwise.Event startEngineSound;
    public AK.Wwise.Event punchSound; //move
    public AK.Wwise.Event balLPunchSound; //move
    public AK.Wwise.Event punchHitSound;
    public AK.Wwise.Event ballPunchHitSound;
    public AK.Wwise.Event punchCounterSound;
    public AK.Wwise.Event ballPunchCounterSound;
    public AK.Wwise.Event grabbingBallSound = new AK.Wwise.Event();
    public float collisionSoundThreshold;
    public AK.Wwise.Event collisionSound;
    public AK.Wwise.Event groundHitSound;
    public AK.Wwise.Event wallHitSound;
    public AK.Wwise.Event parrySound;
    public AK.Wwise.Event parrySuccessSound;
    public AK.Wwise.Event punchTauntSound;
    public AK.Wwise.Event gettingUpSound;
    public AK.Wwise.Event burstSound;
    public AK.Wwise.Event driftStartSound;
    public AK.Wwise.Event driftStopSound;
    public AK.Wwise.Event respawnSound;
    public AK.Wwise.Event brakingSound;
    public AK.Wwise.RTPC engineSpeed;
    public AK.Wwise.RTPC throttle;
    public AK.Wwise.RTPC direction;
    public AK.Wwise.RTPC forceCollision;
    public AK.Wwise.RTPC grounded;


    [Header("ATTACK COLLIDERS")]
    public GameObject punchCollider;
    public GameObject slideCollider;
    public GameObject ballPunchCollider;
    public GameObject ballSlideCollider;

    [Header("ANIMATIONS")] 
    public AnimationCurve curve;

    [Header("CAMERA LAYERS")]
    public List<LayerMask> playerLayer;
    
}
