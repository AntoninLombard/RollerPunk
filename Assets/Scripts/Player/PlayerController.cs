
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerControllerV3 : MonoBehaviour
{

    #region VARIABLES
    
    [Header("DRIVING VALUES")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float forwardAccel;
    [SerializeField] private float gravityStrength;
    [SerializeField] private float turningRate;
    [SerializeField] [Range(0.0f,1.0f)] private float brakingRatio;
    [SerializeField] [Range(0.0f,1.0f)] private float dragRatio;
    [SerializeField] [Range(0.0f,1.0f)] private float intertiaRatio;
    [SerializeField] private float groundAccel;
    
    
    [Header("DRIVING STATE (DO NOT MODIFY)")]
    [SerializeField] private bool isAccelerating ;
    [SerializeField] private bool isBraking;
    [SerializeField] private bool isTurning;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float speed;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float deltaSpeed;
    [SerializeField] [Range(-1.0f,1.0f)] private float steerInput;
    [SerializeField] [Range(-1.0f,1.0f)] private float driveInput;

    
    [Header("INPUT SYSTEM")]
    [SerializeField] private PlayerInput input;
    private InputAction driveAction;
    private InputAction steerAction;
    
    
    [Header("CHARACTER PARTS")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform character;
    [SerializeField] private GameObject ballAnchorPoint;
    [SerializeField] private ParticleSystem particleSystem;

    
    [Header("CHARACTER PHYSICS")] 
    [SerializeField] private CapsuleCollider collider;
    [SerializeField] private float skinWidth;
    [SerializeField] private int maxBounces;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float friction;
    [SerializeField] private float bounciness;
    [SerializeField] private float maxSlopeAngle;
    
    
    
    #endregion
    
    
    void Awake()
    {
        GameManager.Instance.OnPlayerInstantiate(gameObject);
        driveAction = input.actions.FindAction("Driving/Drive");
        steerAction = input.actions.FindAction("Driving/Steer");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void FixedUpdate()
    {
        drag(Time.fixedDeltaTime);
        move(Time.fixedDeltaTime);
        steer(Time.fixedDeltaTime);
        gravity(Time.fixedDeltaTime);
        groundCheck(Time.fixedDeltaTime);
        groundForce(Time.fixedDeltaTime);
        
        currentSpeed = velocity.magnitude;
        rb.velocity = velocity;
    }

    private void OnCollisionEnter(Collision other)
    {
        Vector3 up = character.up;
        velocity -= other.impulse - up * Vector3.Dot(other.impulse,up);
    }
    
    
    
    
    
    private void drag(float time)
     { 
         velocity *= (1 - dragRatio * time);
     }
    
    private void move(float time)
    {
        deltaSpeed = 0;
        driveInput = driveAction.ReadValue<float>();
    
        if (isGrounded)
        {
            if (driveInput > 0)
            {
                deltaSpeed += forwardAccel * time;
            }
            else if (driveInput < 0)
            {
                //deltaSpeed *= (1-brakingRatio * time);
                velocity *= (1 - brakingRatio * time);
            }
        }

        if (isGrounded)
        {
            velocity = velocity * (intertiaRatio * time) + character.forward * (deltaSpeed + (velocity.magnitude * (1f-intertiaRatio * time)));
            currentSpeed = velocity.magnitude;
            
        }
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
    
    void groundForce(float time)
    {
        if (isGrounded)
        {
            velocity -= character.up * (groundAccel * time);
        }
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


    #region PHYSICS
    Vector3 updatePosition()
    {
        Vector3 newPos = character.position;
        int bounces = 0;
        Vector3 remaining = Vector3.zero;
        Vector3 deltaPos = Vector3.zero;
        Vector3 currentVelocity = velocity;
        
        
        while (bounces < maxBounces || CollideAndSlide(currentVelocity,out deltaPos,out remaining,false))
        {
            currentVelocity = remaining;
            bounces++;
        }
        return newPos;
    }

    bool CollideAndSlide(Vector3 currentVelocity,out Vector3 deltaPos,out Vector3 remaining, bool gravityPass)
    {
        Vector3 position = character.position;
        deltaPos = character.position;

        Vector3 capsuleUp = collider.transform.up;
        Vector3 point1 = collider.center + collider.height * capsuleUp;
        Vector3 point2 = collider.center - collider.height * capsuleUp;

        Vector3 dir = currentVelocity.normalized;
        
        if (Physics.CapsuleCast(point1,point2, collider.radius, dir, out RaycastHit hit, currentVelocity.magnitude + skinWidth))
        {
            Vector3 snapPosition = dir * (hit.distance - skinWidth);
            remaining = currentVelocity - snapPosition;
            float slope = Vector3.Angle(character.up, hit.normal);

            if (snapPosition.magnitude <= skinWidth)
            {
                snapPosition = Vector3.zero;
            }

            //Sliding on Slope
            if (slope <= maxSlopeAngle)
            {
                if (gravityPass)
                {
                    remaining = Vector3.zero;
                    deltaPos += snapPosition;
                    return true;
                }
                remaining = Vector3.ProjectOnPlane(remaining,hit.normal).normalized * remaining.magnitude;
            }
            //Colliding with wall
            else
            {
                remaining = Vector3.ProjectOnPlane(remaining,hit.normal) * friction;
            }

        }
        
        //No collision the object can move in a straight line
        remaining = Vector3.zero;
        deltaPos += currentVelocity;
        return false;
    }
    
    #endregion
    

}
