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
    public float gripAccel = 0.0f;
    [Range(0.0f,1.0f)] public float inertiaRatio = 0.0f;
    [Range(0.0f,1.0f)] public float brakingRatio = 0;
    [Range(0.0f,1.0f)] public float drag;
    [Range(0.0f,1.0f)] public float airDrag;
    [Range(0.2f,1.0f)] public float ballAccelMultiplier;
    [Range(0.2f,1.0f)] public float ballMaxSpeedMultipier;
    
    [Header("COMBAT VALUES")] 
    public float counterWindow;
    public float actionsCooldown;
    public float stunDuration;
    public float invincibilityDuration;
    public float slidingDrag;
    
    
    [Header("SOUND")] 
    public AK.Wwise.Event startEngineSound;
    public AK.Wwise.Event punchSound; //move
    public AK.Wwise.Event balLPunchSound; //move
    public AK.Wwise.Event punchHitSound;
    public AK.Wwise.Event ballPunchHitSound;
    public AK.Wwise.Event punchCounterSound;
    public AK.Wwise.Event ballPunchCounterSound;
    public AK.Wwise.Event grabbingBallSound = new AK.Wwise.Event();
    public AK.Wwise.Event collisionSound;
    public AK.Wwise.Event wallHitSound;
    public AK.Wwise.Event parrySound;
    public AK.Wwise.Event parrySuccessSound;
    public AK.Wwise.Event punchAfterParrySound;
    public AK.Wwise.Event punchTauntSound;
    public AK.Wwise.RTPC engineSpeed;
    public AK.Wwise.RTPC vehicleSpeed;
    public AK.Wwise.RTPC throttle;
    public AK.Wwise.RTPC direction;
    
    
    [Header("ATTACK COLLIDERS")]
    public GameObject punchCollider;
    public GameObject slideCollider;
    public GameObject ballPunchCollider;
    public GameObject ballSlideCollider;
}
