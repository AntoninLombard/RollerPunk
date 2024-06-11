using System.Collections;
using UnityEngine;
using AK.Wwise;


public class PlayerController2 : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private PlayerControllerData controllerData;
    

    [field: Header("DRIVING STATE")]
    [field: SerializeField] public bool isAccelerating { get; private set; }
    [field: SerializeField] public bool isBraking { get; private set; }
    [field: SerializeField] public bool isTurning { get; private set; }

    [field: SerializeField] public bool isBoosting { get; private set; }
    [field: SerializeField] public bool isGrounded { get; private set; }
    [field: SerializeField] public bool isDrifting { get; private set; }
    [SerializeField] private int driftingSide;
    [SerializeField] private float speed = 0;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float driftInputValue = 0;
    [SerializeField] private float speedRatio;
    

    
    [Header("CHARACTER PARTS")]
    [SerializeField] public Rigidbody rb;
    
    [field: Header("GROUND DETECTION")]
    [field: SerializeField] [field: Range(0.1f,1.0f)] public float groundRange { get; private set; }
    [SerializeField] private Vector3 groundNormal;

    [Header("SOUND")] 
    [SerializeField] RTPC engineSpeed;
    
    [SerializeField] private Player player;
    private Vector3 previousForward;
    private float driftDuration;
    private static readonly int Moving = Animator.StringToHash("Moving");
    private static readonly int Brake = Animator.StringToHash("Brake");
    private static readonly int Direction = Animator.StringToHash("Direction");
    private static readonly int Speed = Animator.StringToHash("Speed");

    #endregion

    #region UNITY FUNCTIONS
    
    // Start is called before the first frame update
    void Start()
    {

        controllerData.startEngineSound.Post(gameObject);
        maxSpeed = controllerData.maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        speedRatio = Vector3.Dot(rb.velocity,player.character.forward) / player.data.maxSpeed;
        if (isDrifting)
            driftDuration += Time.deltaTime;
        player.anime.animator.SetBool(Moving,speed > 1);
        player.anime.animator.SetFloat(Direction,player.input.steerInput);
        player.anime.animator.SetFloat(Brake,player.input.brakeInput);
        player.anime.animator.SetFloat(Speed,speedRatio);
        //steer(Time.deltaTime);
        //groundCheck(Time.deltaTime);
        player.virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(player.virtualCamera.m_Lens.FieldOfView,player.data.minFOV + (player.data.maxFOV -player.data.minFOV) * speedRatio,5f * Time.deltaTime);
        controllerData.throttle.SetValue(gameObject, player.input.driveInput);
        controllerData.direction.SetValue(gameObject, player.input.steerInput);
        
        player.anime.animator.SetBool("AirTime", !isGrounded);
        
        isBraking = player.input.brakeInput != 0;
    }

    private void FixedUpdate()
    {
        Vector3 currentVelocity = rb.velocity;
        //Quaternion deltaRot = Quaternion.FromToRotation(previousForward, player.character.forward);
        //currentVelocity = (deltaRot * currentVelocity) * (1 - controllerData.inertiaRatio) + currentVelocity * controllerData.inertiaRatio ;
        Vector3 deltaVelocity = Vector3.zero;
        
        
        // deltaVelocity += drag(currentVelocity,Time.fixedDeltaTime);
        // deltaVelocity += friction(currentVelocity, Time.fixedDeltaTime);
        deltaVelocity += move(currentVelocity,Time.fixedDeltaTime);
        deltaVelocity += boost(Time.fixedDeltaTime);
        groundCheck(Time.fixedDeltaTime);
        steer(Time.fixedDeltaTime);
        gravity();
        gripForce();
        
        
        rb.AddForce(deltaVelocity ,ForceMode.VelocityChange);
        brake(Time.fixedDeltaTime);
        currentVelocity = rb.velocity;
        speed = Vector3.Dot(currentVelocity, player.character.forward);
        
        
        if (speed > maxSpeed * (!player.combat.isHoldingBall? 1 : controllerData.ballMaxSpeedMultipier))
        {
            rb.AddForce(player.character.forward * (maxSpeed * (!player.combat.isHoldingBall? 1 : controllerData.ballMaxSpeedMultipier)) - currentVelocity,ForceMode.VelocityChange);
        }

        speed = rb.velocity.magnitude;
        engineSpeed.SetValue(gameObject, speed);
    }
    
    #endregion

    #region DRIVING FUNCTIONS


    // private Vector3 drag(Vector3 velocity,float time)
    // {
    //     if (isGrounded)
    //         return -velocity * (controllerData.drag * time);
    //     return -velocity * (controllerData.airDrag * time);
    // }
    
    // private Vector3 drag(Vector3 velocity,float time)
    // {
    //     return -velocity.sqrMagnitude * controllerData.airDrag * time * velocity.normalized;
    // }
    //
    // private Vector3 friction(Vector3 velocity,float time)
    // {
    //     if (!isGrounded)
    //         return Vector3.zero;
    //     Vector3 forces = rb.GetAccumulatedForce() + (isGrounded? -player.character.up * controllerData.gripAccel : Vector3.down * controllerData.gravityStrength);
    //     return - controllerData.friction * Vector3.Dot(groundNormal,forces) * time * velocity.normalized;
    // }
    
    private Vector3 move(Vector3 currentVelocity,float time)
    {
        Vector3 deltaVelocity = Vector3.zero;

        if (!isGrounded || player.combat.isStunned)
        {
            return deltaVelocity;
        }
        deltaVelocity += player.character.forward * ((isDrifting ? driftInputValue : player.input.driveInput) * controllerData.forwardAccel * (!player.combat.isHoldingBall? 1f : controllerData.ballAccelMultiplier) * time);

        // {
        //     case > 0f:
        //         isAccelerating = true;
        //         deltaVelocity += player.character.forward * (player.input.driveInput * controllerData.forwardAccel * (!player.combat.isHoldingBall? 1f : controllerData.ballAccelMultiplier) * Time.fixedDeltaTime);
        //         break;
        //     case < 0f:
        //         isAccelerating = false;
        //         deltaVelocity -= currentVelocity * (controllerData.brakingRatio * Time.fixedDeltaTime);
        //         break;
        //     default:
        //         isAccelerating = false;
        //         break;
        // }
            
        return deltaVelocity;
    }

    private void brake(float time)
    {
        if (isBraking)
            rb.AddForce(-rb.velocity * (controllerData.brakingRatio * time * player.input.brakeInput),ForceMode.VelocityChange);
    }

    private void steer(float time)
    {
        if (isDrifting)
        {
            player.character.rotation = Quaternion.Lerp(player.character.rotation, Quaternion.AngleAxis(
                (player.input.steerInput + driftingSide)/2 *  controllerData.driftTurnRate + controllerData.driftTurnOffset * driftingSide,player.character.up) * player.character.rotation, time * 5f);
        } else if (player.input.steerInput != 0f)
        {
            float angle = player.input.steerInput * (controllerData.minTurningRate + (1-speedRatio) * (controllerData.maxTurningRate - controllerData.minTurningRate));
            player.character.rotation = Quaternion.Lerp(player.character.rotation, Quaternion.AngleAxis(angle,player.character.up) * player.character.rotation, time * 5f);
        }
    }

    private void gravity()
    {
        if(!isGrounded)
            rb.AddForce(Vector3.down * (controllerData.gravityStrength * Time.fixedDeltaTime),ForceMode.VelocityChange);
    }
    
    private void gripForce()
    {
        if(isGrounded)
            rb.AddForce(-player.character.up * (controllerData.gripAccel * Time.fixedDeltaTime),ForceMode.VelocityChange);
    }
    
    void groundCheck(float time)
    {
        Vector3 pos = player.character.position;
        Vector3 up = player.character.up;
        Vector3 right = player.character.right;
        Vector3 forward = player.character.forward;

        int count = 0;
        Vector3 normal = Vector3.zero;
        if (Physics.Raycast(pos, -player.character.up, out var hitUnder, groundRange))
        {
            count += 3;
            normal += hitUnder.normal*3;
        }
        if (Physics.Raycast(pos, -up - right, out var hitLeft, groundRange))
        {
            count++;
            normal += hitLeft.normal;
        }
        if (Physics.Raycast(pos, -up + right, out var hitRight, groundRange))
        {
            count++;
            normal += hitRight.normal;
        }
        if (Physics.Raycast(pos, -up + forward, out var hitFront, groundRange))
        {
            count++;
            normal += hitFront.normal;
        }
        if (Physics.Raycast(pos, -up -forward, out var hitBack, groundRange))
        {
            count++;
            normal += hitBack.normal;
        }

        if (count != 0)
        {
            isGrounded = true;
            normal /= count;
            float dot = Vector3.Dot(normal, player.character.up);
            groundNormal = normal;
            alignCharToNormal(normal, time);
        }
        else if (Physics.Raycast(player.character.position, Vector3.down, out var hitDown, 0.5f))
        {
            isGrounded = true;
            alignCharToNormal(hitDown.normal, time/2);
        }
        else
        {
            if (isGrounded && isDrifting)
            {
                CancelDrift(false);
            }
            isGrounded = false;
            alignCharToNormal(Vector3.up, time/5);
        }
    }

    Vector3 boost(float time)
    {
        if (isBoosting)
            return player.character.forward * (controllerData.boostAccel * time * controllerData.ballAccelMultiplier);
        return Vector3.zero;
    }


    void alignCharToNormal(Vector3 normal,float time)
    {
        Vector3 up = Vector3.Lerp(player.character.up, normal, 3.0f * time);
        Vector3 forward = (player.character.forward - up * Vector3.Dot(player.character.forward, up)).normalized;
        player.character.rotation = Quaternion.LookRotation(forward, up);
    }
    
    void OnDrawGizmosSelected()
    {
        Vector3 pos = player.character.position;
        Vector3 up = player.character.up;
        Vector3 right = player.character.right;
        Vector3 forward = player.character.forward;
        
        Gizmos.color = Color.red;
        if (Physics.Raycast(player.character.position, -up, out var hitUnder, groundRange))
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(pos, pos + -player.character.up * groundRange);

        
        Gizmos.color = Color.red;
        if (Physics.Raycast(player.character.position, -up - right, out var hitLeft, groundRange))
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(pos, pos + (-up - right).normalized * groundRange);
        
        
        Gizmos.color = Color.red;
        if (Physics.Raycast(player.character.position, -up + right, out var hitRight, groundRange))
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(pos, pos + (-up + right).normalized * groundRange);
        
        Gizmos.color = Color.red;
        if (Physics.Raycast(player.character.position, -up + forward, out var hitFront, groundRange))
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(pos, pos + (-up + forward).normalized * groundRange);
        
        Gizmos.color = Color.red;
        if (Physics.Raycast(player.character.position, -up -forward, out var hitBack, groundRange))
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(pos, pos + (-up - forward).normalized * groundRange);
    }

    public void TeleportPlayer(Vector3 position, Quaternion rotation)
    {
        rb.velocity = Vector3.zero;
        player.character.position = position;
        player.character.rotation = rotation;
    }
    
    
    public void TogglePlayerFreeze(bool isFreezed)
    {
        rb.isKinematic = isFreezed;
        rb.interpolation = isFreezed ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate;
    }

    public void StartDrift()
    {
        if (!player.combat.isBusy)
        {
            if(player.input.steerInput != 0 && player.controller.speed > 0 && isGrounded)
            {
                isDrifting = true;
                driftingSide = player.input.steerInput > 0 ? 1 : -1;
                if (driftingSide >0)
                {
                    player.anime.animator.SetBool("Drift.R", true);
                    
                }
                else
                {
                    player.anime.animator.SetBool("Drift.L", true);
                } 
                driftInputValue  = player.input.driveInput;
                player.data.driftStartSound.Post(gameObject);
            }
        }
    }

    public void CancelDrift(bool canBoost = true)
    {
        if (isDrifting)
        {
            if(driftingSide >0)
            {
                player.anime.animator.SetBool("Drift.R", false);
            }
            else
            {
                player.anime.animator.SetBool("Drift.L", false);
            }
            if (driftDuration > controllerData.driftDurationForBoost)
                StartBoost();
            isDrifting = false;
            driftDuration = 0;
            driftingSide = 0;
            driftInputValue = 0f;
            player.data.driftStopSound.Post(gameObject);

        }
    }


    public void StartBoost()
    {
        StartCoroutine(Boost());
    }
    private IEnumerator Boost()
    {
        controllerData.burstSound.Post(gameObject);
        isBoosting = true;
        maxSpeed =  controllerData.boostMaxSpeed;
        yield return new WaitForSeconds(controllerData.boostDuration);
        isBoosting = false;
        maxSpeed = controllerData.maxSpeed;
    }
    
    
    #endregion
    
}
