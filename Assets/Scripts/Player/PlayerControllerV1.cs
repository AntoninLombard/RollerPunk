using UnityEngine;
using AK.Wwise;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region VARIABLES

    [Header("DRIVING VALUES")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float forwardAccel = 0;
    [SerializeField] [Range(0.0f,1.0f)] private float brakingRatio = 0;
    [SerializeField] private float turningRate = 0;
    [SerializeField] private float gravityStrength = 0.0f;
    [SerializeField] [Range(0.0f,1.0f)] private float drag;


    [field: Header("DRIVING STATE (DO NOT MODIFY)")]
    [field: SerializeField] public bool isAccelerating { get; private set; }
    [field: SerializeField] public bool isBraking { get; private set; }
    [field: SerializeField] public bool isTurning { get; private set; }
    [field: SerializeField] public bool isGrounded { get; private set; }   

    [SerializeField] private float speed = 0;
    [SerializeField] private float currentSpeed = 0;


    [SerializeField] private Transform character;
    
    [Header("INPUT SYSTEM")]
    [SerializeField] private PlayerInput input;
    private InputAction driveAction;
    private InputAction steerAction;
    
    [Header("PLAYER INPUTS")]
    [SerializeField] [Range(-1.0f,1.0f)] private float steerInput = 0;
    [SerializeField] [Range(-1.0f,1.0f)] private float driveInput = 0;
    [SerializeField] private bool punchInput;
    [SerializeField] private bool slideInput;
    [SerializeField] private bool fortifyInput;
    
    
    [Header("SOUND")] 
    [SerializeField] private AK.Wwise.Event startEngineSound;
    [SerializeField] RTPC engineSpeed;
    #endregion
    
    
    
    
    
    [SerializeField] private Rigidbody rb;
    
    
    #region UNITY FUNCTIONS
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
        Vector3 _previousPos = rb.transform.position;
        character.position = rb.position - character.up*0.5f;
        //rb.transform.position = _previousPos;

        steerInput = steerAction.ReadValue<float>();
        driveInput = driveAction.ReadValue<float>();
        
        
        //Apply drag force
        currentSpeed -= currentSpeed * drag * Time.deltaTime;
        if (!isGrounded)
        {
            
        }
        else if (driveInput > 0f)
        {
            isBraking = false;
            isAccelerating = true;
            currentSpeed = currentSpeed + forwardAccel * Time.deltaTime;
        }
        else if(driveInput < 0f)
        {
            isBraking = true;
            isAccelerating = false;
            currentSpeed = currentSpeed * (1-brakingRatio * Time.deltaTime);
        } else
        {
            isBraking = false;
            isAccelerating = false;
        }

        if (steerInput != 0)
        {
            isTurning = true;
        }
        else
        {
            isTurning = false;
        }
        
        if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }
        
        groundCheck(Time.deltaTime);
        engineSpeed.SetValue(gameObject,currentSpeed);
    }
    
    void FixedUpdate()
    {
        if (isGrounded)
        {
            rb.AddForce(character.forward * (currentSpeed * rb.mass));

        }
        //rb.AddForce(character.forward * currentSpeed, ForceMode.Acceleration);
        //rb.velocity = transform.forward * currentSpeed + Vector3.down * gravityStrength;

        
        /*//Speeding up the player
        if (isAccelerating & !isBraking)
        {
            rb.AddForce(transform.forward * (forwardAccel * driveInput), ForceMode.Acceleration);
        }

        
        //Slowing down the player when braking
        if (isBraking && rb.velocity.magnitude > 0)
        {
            rb.AddForce(transform.forward * (brakingAccel * rb.velocity.magnitude * driveInput));
        }*/

        
        
        //Turning the player toward the new direction
        if (isTurning)
        {
            character.rotation = Quaternion.Euler(Vector3.Lerp(character.rotation.eulerAngles, character.rotation.eulerAngles + new Vector3(0, steerInput * turningRate, 0), Time.fixedDeltaTime * 5f));
        }


        //groundCheck();
        
        //Applying gravity force down ward
        if (!isGrounded)
        {
            //rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
        }

        speed = rb.velocity.magnitude;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Draw the suspension
        Gizmos.DrawLine(
            transform.position + Vector3.up *3f, transform.position + rb.velocity + Vector3.up *3f
        );
        
        RaycastHit hit;
        if (Physics.Raycast(character.position, -character.up,out hit,0.5f))
        {
            Gizmos.color = Color.green;
        }
        //Draw the suspension
        Gizmos.DrawLine(character.position, character.position + -character.up * 0.5f);
    }
    #endregion

    #region DRIVING_FUNCTIONS

    void groundCheck(float time)
    {
        //Adjust character model to the surface normal it's on
        if (Physics.Raycast(character.position, -character.up, out var hit, 0.5f))
        {
            isGrounded = true;
            //METHOD 1
            //Vector3 rot = character.rotation.eulerAngles;
            //character.up = Vector3.Lerp(character.up, hit.normal, time * 2.0f);
            //character.Rotate(0, rot.y, 0);
            
            
            //METHOD 2
            //Quaternion rot = Quaternion.FromToRotation(character.up,hit.normal) * transform.rotation;
            //character.rotation = Quaternion.Slerp(character.rotation, rot * character.rotation, 10.0f * time);
            //character.rotation = rot;



            //METHOD 3
            alignCharToNormal(hit.normal, time);
        }
        else if (Physics.Raycast(character.position, Vector3.down, out var hitDown, 1.0f))
        {
            alignCharToNormal(hitDown.normal, time);
        }
        else
        {
            isGrounded = false;
            alignCharToNormal(Vector3.up, time);
        }
    }


    void alignCharToNormal(Vector3 normal,float time)
    {
        Vector3 up = Vector3.Lerp(character.up, normal, 3.0f * time);
        Vector3 forward = (character.forward - up * Vector3.Dot(character.forward, up)).normalized;
        character.rotation = Quaternion.LookRotation(forward, up);
    }

    #endregion



    #region INPUT_CALLBACKS

    public void Punch(InputAction.CallbackContext context)
    {
        punchInput = !punchInput;
    }

    public void Slide(InputAction.CallbackContext context)
    {
        slideInput = !slideInput;
    }

    public void Fortify(InputAction.CallbackContext context)
    {
        fortifyInput = !fortifyInput;
    }

    #endregion

}

