using AK.Wwise;
using UnityEngine;
using UnityEngine.InputSystem;
using Event = AK.Wwise.Event;

public class PlayerController2 : MonoBehaviour
{
    #region VARIABLES

    [Header("DRIVING VALUES")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float forwardAccel = 0;
    [SerializeField] [Range(0.0f,1.0f)] private float brakingRatio = 0;
    [SerializeField] private float turningRate = 0;
    [SerializeField] private float gravityStrength = 0.0f;
    [SerializeField] [Range(0.0f,1.0f)] private float dragRatio;
    [SerializeField] private float groundAccel;
    
    [Header("DRIVING STATE (DO NOT MODIFY)")]
    [SerializeField] private bool isAccelerating = false;
    [SerializeField] private bool isBraking = false;
    [SerializeField] private bool isTurning = false;
    [SerializeField] private bool isGrounded = false;

    [SerializeField] private float speed = 0;
    [SerializeField] private float currentSpeed = 0;
    [SerializeField] [Range(-1.0f,1.0f)] private float steerInput = 0;
    [SerializeField] [Range(-1.0f,1.0f)] private float driveInput = 0;

    [SerializeField] private Vector3 velocity;


    [SerializeField] private Transform character;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerInput input;

    private InputAction driveAction;
    private InputAction steerAction;

    [Header("SOUND")] 
    [SerializeField] private Event startEngineSound;
    [SerializeField] RTPC engineSpeed;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        driveAction = input.actions.FindAction("Driving/Drive");
        steerAction = input.actions.FindAction("Driving/Steer");
        startEngineSound.Post(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        velocity = Vector3.zero;
        drag(Time.deltaTime);
        move(Time.deltaTime);
        steer(Time.deltaTime);
        gravity(Time.deltaTime);
        groundCheck(Time.deltaTime);
        groundForce(Time.deltaTime);
        engineSpeed.SetValue(gameObject,currentSpeed);
    }

    private void FixedUpdate()
    {
        //rb.velocity = velocity;
        rb.AddForce(velocity,ForceMode.Acceleration);
    }


    #region MY FUNCTIONS


    private void drag(float time)
    {
        currentSpeed *= (1-dragRatio * time);
    }

    private void move(float time)
    {

        driveInput = driveAction.ReadValue<float>();

        if (isGrounded)
        {
            if (driveInput > 0)
            {
                currentSpeed += forwardAccel * time;
            }
            else if (driveInput < 0)
            {
                currentSpeed *= (1-brakingRatio * time);
            }
        }
        if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }
        else if (currentSpeed < 0.001)
        {
            currentSpeed = 0;
        }

        velocity = character.forward * currentSpeed;
    }

    private void steer(float time)
    {
        steerInput = steerAction.ReadValue<float>();
        if (steerInput != 0f)
        {
            character.rotation = Quaternion.Euler(Vector3.Lerp(character.rotation.eulerAngles, character.rotation.eulerAngles + new Vector3(0, steerInput * turningRate, 0), time * 5f));
        }
    }

    private void gravity(float time)
    {
        velocity += Vector3.down * (gravityStrength * time);
    }
    
    
    void groundCheck(float time)
    {
        Vector3 pos = character.position;
        Vector3 up = character.up;
        Vector3 right = character.right;
        Vector3 forward = character.forward;

        int count = 0;
        Vector3 normal = Vector3.zero;
        if (Physics.Raycast(pos, -character.up, out var hitUnder, 0.5f))
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
        else if (Physics.Raycast(character.position, Vector3.down, out var hitDown, 0.5f))
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


    void alignCharToNormal(Vector3 normal,float time)
    {
        Vector3 up = Vector3.Lerp(character.up, normal, 3.0f * time);
        Vector3 forward = (character.forward - up * Vector3.Dot(character.forward, up)).normalized;
        character.rotation = Quaternion.LookRotation(forward, up);
    }
    
    void OnDrawGizmosSelected()
    {
        Vector3 pos = character.position;
        Vector3 up = character.up;
        Vector3 right = character.right;
        Vector3 forward = character.forward;
        
        Gizmos.color = Color.red;
        if (Physics.Raycast(character.position, -up, out var hitUnder, 0.5f))
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(pos, pos + -character.up * 0.5f);

        
        Gizmos.color = Color.red;
        if (Physics.Raycast(character.position, -up - right, out var hitLeft, 0.5f))
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(pos, pos + (-up - right).normalized * 0.5f);
        
        
        Gizmos.color = Color.red;
        if (Physics.Raycast(character.position, -up + right, out var hitRight, 0.5f))
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(pos, pos + (-up + right).normalized * 0.5f);
        
        Gizmos.color = Color.red;
        if (Physics.Raycast(character.position, -up + forward, out var hitFront, 0.5f))
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(pos, pos + (-up + forward).normalized * 0.5f);
        
        Gizmos.color = Color.red;
        if (Physics.Raycast(character.position, -up -forward, out var hitBack, 0.5f))
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(pos, pos + (-up - forward).normalized * 0.5f);
    }

    void groundForce(float time)
    {
        if (isGrounded)
        {
            velocity -= character.up * (groundAccel * time);
        }
    }
    
    #endregion
}
