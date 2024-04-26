using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerControllerData : ScriptableObject
{
    [Header("DRIVING VALUES")]
    public float maxSpeed;
    public float forwardAccel = 0;
    [Range(0.0f,1.0f)] public  float brakingRatio = 0;
    public float turningRate = 0;
    public float gravityStrength = 0.0f;
    [Range(0.0f,1.0f)] public float drag;
    
    [Header("COMBAT VALUES")] 
    public float counterWindow;
    public float actionsCooldown;
    
    [Header("SOUND")] 
    public AK.Wwise.Event startEngineSound;
    public AK.Wwise.Event punchSound;
    public AK.Wwise.Event slideSound;
    public AK.Wwise.Event balLPunchSound;
    public AK.Wwise.Event ballSlideSound;
    public AK.Wwise.Event punchHitSound;
    public AK.Wwise.Event slideHitSound;
    public AK.Wwise.Event ballPunchHitSound;
    public AK.Wwise.Event ballSlideHitSound;
    public AK.Wwise.Event punchCounterSound;
    public  AK.Wwise.Event slideCounterSound;
    public AK.Wwise.Event ballPunchCounterSound;
    public AK.Wwise.Event ballSlideCounterSound;
    public AK.Wwise.RTPC engineSpeed;
    public AK.Wwise.RTPC vehicleSpeed;
    public AK.Wwise.RTPC throttle;
    public AK.Wwise.RTPC direction;
    public AK.Wwise.Switch onGround;
    public AK.Wwise.Switch offGround;
    
    [Header("ATTACK COLLIDERS")]
    public GameObject punchCollider;
    public GameObject slideCollider;
    public GameObject ballPunchCollider;
    public GameObject ballSlideCollider;
}
