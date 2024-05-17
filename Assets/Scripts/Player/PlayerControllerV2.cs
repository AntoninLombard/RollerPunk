using System;
using System.Collections;
using UnityEngine;
using AK.Wwise;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController2 : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private PlayerControllerData controllerData;
    

    [field: Header("DRIVING STATE")]
    [field: SerializeField] public bool isAccelerating { get; private set; }
    [field: SerializeField] public bool isBraking { get; private set; }
    [field: SerializeField] public bool isTurning { get; private set; }
    [field: SerializeField] public bool isGrounded { get; private set; }   
    [SerializeField] private float speed = 0;
    [SerializeField] private Vector3 targetForward;
    
    
    [Header("INPUT SYSTEM")]
    [SerializeField] private PlayerInput input;
    private InputAction driveAction;
    private InputAction reverseAction;
    private InputAction steerAction;
    
    [field: Header("PLAYER INPUTS")]
    [field: SerializeField] [field: Range(-1.0f,1.0f)] public float steerInput { get; private set; }
    [field: SerializeField] [field: Range(-1.0f,1.0f)] public float driveInput { get; private set; }
    
    [Header("CHARACTER PARTS")]
    [SerializeField] public Rigidbody rb;
    

    [Header("SOUND")] 
    [SerializeField] RTPC engineSpeed;
    
    [SerializeField] private Player player;
    private Vector3 previousForward;
    #endregion

    #region UNITY FUNCTIONS
    
    // Start is called before the first frame update
    void Start()
    {
        targetForward = player.character.forward;
        driveAction = input.actions.FindAction("Driving/Drive");
        reverseAction = input.actions.FindAction("Driving/Reverse");
        steerAction = input.actions.FindAction("Driving/Steer");
        controllerData.startEngineSound.Post(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        steerInput = steerAction.ReadValue<float>();
        driveInput = driveAction.ReadValue<float>() - reverseAction.ReadValue<float>();
        player.anime.animator.SetBool("Moving",speed > 1);
        player.anime.animator.SetFloat("Direction",steerInput);
        player.anime.animator.SetFloat("Speed",Vector3.Dot(rb.velocity,player.character.forward) / player.data.maxSpeed);
        steer(Time.deltaTime);
        groundCheck(Time.deltaTime);
        controllerData.throttle.SetValue(gameObject, driveInput);
        controllerData.direction.SetValue(gameObject, steerInput);
    }

    private void FixedUpdate()
    {
        Vector3 currentVelocity = rb.velocity;
        //Quaternion deltaRot = Quaternion.FromToRotation(previousForward, player.character.forward);
        //currentVelocity = (deltaRot * currentVelocity) * (1 - controllerData.inertiaRatio) + currentVelocity * controllerData.inertiaRatio ;
        Vector3 deltaVelocity = Vector3.zero;
        
        
        deltaVelocity += drag(currentVelocity,Time.fixedDeltaTime);
        deltaVelocity += move(currentVelocity,Time.fixedDeltaTime);
        deltaVelocity += sliding(targetForward,Time.deltaTime); // TODO change direction 
        groundCheck(Time.deltaTime);
        gravity();
        gripForce();
        
        
        rb.AddForce(deltaVelocity + currentVelocity - rb.velocity ,ForceMode.VelocityChange);
        
        currentVelocity = rb.velocity;

        if (currentVelocity.magnitude > controllerData.maxSpeed * (!player.combat.isHoldingBall? 1 : controllerData.ballMaxSpeedMultipier))
        {
            rb.AddForce(currentVelocity.normalized * (controllerData.maxSpeed * (!player.combat.isHoldingBall? 1 : controllerData.ballMaxSpeedMultipier)) - currentVelocity,ForceMode.VelocityChange);
        }

        speed = rb.velocity.magnitude;
        engineSpeed.SetValue(gameObject, speed);
        previousForward = player.character.forward;
    }
    
    #endregion

    #region DRIVING FUNCTIONS


    private Vector3 drag(Vector3 velocity,float time)
    {
        if (isGrounded)
            return -velocity * (controllerData.drag * time);
        return -velocity * (controllerData.airDrag * time);
    }

    private Vector3 move(Vector3 currentVelocity,float time)
    {
        Vector3 deltaVelocity = Vector3.zero;

        if (!isGrounded)
        {
        }
        else if(!player.combat.isStunned)
        {
            switch (driveInput)
            {
                case > 0f:
                    isBraking = false;
                    isAccelerating = true;
                    deltaVelocity += player.character.forward * (driveInput * controllerData.forwardAccel * (!player.combat.isHoldingBall? 1f : controllerData.ballAccelMultiplier) * Time.fixedDeltaTime);
                    break;
                case < 0f:
                    isBraking = true;
                    isAccelerating = false;
                    deltaVelocity -= currentVelocity * (controllerData.brakingRatio * Time.fixedDeltaTime);
                    break;
                default:
                    isBraking = false;
                    isAccelerating = false;
                    break;
            }
        
        }
        return deltaVelocity;
    }

    private void steer(float time)
    {
        steerInput = steerAction.ReadValue<float>();
        if (steerInput != 0f)
        {
            //player.character.rotation = Quaternion.Euler(Vector3.Lerp(player.character.rotation.eulerAngles, player.character.rotation.eulerAngles + new Vector3(0, steerInput * controllerData.turningRate, 0), time * 5f));
            player.character.rotation = Quaternion.Lerp(player.character.rotation, Quaternion.AngleAxis(steerInput * controllerData.turningRate,player.character.up) * player.character.rotation, time * 5f);
            //player.character.rotation = player.character.rotation * Quaternion.AngleAxis(steerInput * controllerData.turningRate,player.character.up);
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
        if (Physics.Raycast(pos, -player.character.up, out var hitUnder, 0.5f))
        {
            count += 3;
            normal += hitUnder.normal*3;
        }
        if (Physics.Raycast(pos, -up - right, out var hitLeft, 0.5f))
        {
            count++;
            normal += hitLeft.normal;
        }
        if (Physics.Raycast(pos, -up + right, out var hitRight, 0.5f))
        {
            count++;
            normal += hitRight.normal;
        }
        if (Physics.Raycast(pos, -up + forward, out var hitFront, 0.5f))
        {
            count++;
            normal += hitFront.normal;
        }
        if (Physics.Raycast(pos, -up -forward, out var hitBack, 0.5f))
        {
            count++;
            normal += hitBack.normal;
        }

        if (count != 0)
        {
            isGrounded = true;
            normal /= count;
            alignCharToNormal(normal, time);
        }
        else if (Physics.Raycast(player.character.position, Vector3.down, out var hitDown, 0.5f))
        {
            isGrounded = true;
            alignCharToNormal(hitDown.normal, time/2);
        }
        else
        {
            isGrounded = false;
            alignCharToNormal(Vector3.up, time/5);
        }
    }
    
    
    private Vector3 sliding(Vector3 dir,float time)
    {
        // if (player.combat.isSliding)
        //     return -dir  * (controllerData.slidingDrag * time);
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
        if (Physics.Raycast(player.character.position, -up, out var hitUnder, 0.5f))
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(pos, pos + -player.character.up * 0.5f);

        
        Gizmos.color = Color.red;
        if (Physics.Raycast(player.character.position, -up - right, out var hitLeft, 0.5f))
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(pos, pos + (-up - right).normalized * 0.5f);
        
        
        Gizmos.color = Color.red;
        if (Physics.Raycast(player.character.position, -up + right, out var hitRight, 0.5f))
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(pos, pos + (-up + right).normalized * 0.5f);
        
        Gizmos.color = Color.red;
        if (Physics.Raycast(player.character.position, -up + forward, out var hitFront, 0.5f))
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(pos, pos + (-up + forward).normalized * 0.5f);
        
        Gizmos.color = Color.red;
        if (Physics.Raycast(player.character.position, -up -forward, out var hitBack, 0.5f))
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(pos, pos + (-up - forward).normalized * 0.5f);
    }
    
    #endregion
    
}
