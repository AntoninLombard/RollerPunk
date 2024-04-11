using System.Collections;
using UnityEngine;
using AK.Wwise;
using UnityEngine.Events;
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


    [Header("CHARACTER STATE")]
    [SerializeField] private bool isPunching;
    [SerializeField] private bool isSliding;
    
    
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

    
    [Header("Events")] 
    [SerializeField] public UnityEvent<GameObject> OnHitByPunch;
    [SerializeField] public UnityEvent<GameObject> OnHitbySlide;
    [SerializeField] public UnityEvent<GameObject> OnHitbByBall;
    
    
    [Header("CHARACTER PARTS")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform character;
    [SerializeField] private GameObject ballAnchorPoint;
    [SerializeField] private ParticleSystem particleSystem;

    [Header("ATTACK COLLIDERS")]
    [SerializeField] private GameObject punchCollider;
    [SerializeField] private GameObject slideCollider;
    [SerializeField] private GameObject ballCollider;
    
    #endregion
    
    #region UNITY FUNCTIONS

    private void Awake()
    {
        OnHitByPunch.AddListener(onHitByPunch);
        OnHitbySlide.AddListener(onHitBySlide);
        OnHitbByBall.AddListener(onHitByBall);
    }


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
        //Vector3 _previousPos = rb.transform.position;
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



    #region INPUT EVENT CALLBACKS

    public void onPunch(InputAction.CallbackContext context)
    {
        if (!isSliding && !isPunching && isGrounded)
        {
            StartCoroutine(Punch());
        }
    }

    public void onSlide(InputAction.CallbackContext context)
    {
        if (!isSliding && !isPunching && isGrounded)
        {
            StartCoroutine(Slide());
        }
    }

    public void onFortify(InputAction.CallbackContext context)
    {
        fortifyInput = !fortifyInput;
    }

    #endregion


    #region GAMEPLAY EVENTS CALLBACKS

    void onHitBySlide(GameObject source)
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.yellow;
        particleSystem.Play();
    }
    
    void onHitByPunch(GameObject source)
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.green;
        particleSystem.Play();
    }
    
    void onHitByBall(GameObject source)
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.red;
        particleSystem.Play();
    }

    #endregion


    #region ACTIONS COROUTINES

    IEnumerator Punch()
    {
        isPunching = true;
        GameObject collider = Instantiate(punchCollider,character);
        ColliderBox box = collider.GetComponent<ColliderBox>();
        box.SetSource(character.gameObject);
        box.SetType(ColliderBox.ColliderType.Punch);
        collider.transform.localPosition = character.position + character.up + character.forward * 0.5f;
        yield return new WaitForSeconds(1);
        Destroy(collider);
        isPunching = false;
    }
    
    IEnumerator Slide()
    {
        isSliding = true;
        GameObject collider = Instantiate(slideCollider,character);
        ColliderBox box = collider.GetComponent<ColliderBox>();
        box.SetSource(character.gameObject);
        box.SetType(ColliderBox.ColliderType.Slide);
        collider.transform.position = character.position + character.up + character.forward * 0.5f;
        yield return new WaitForSeconds(1);
        Destroy(collider);
        isSliding = false;
    }
    
    #endregion

}

