using System.Collections;
using UnityEngine;
using AK.Wwise;
using Unity.VisualScripting;
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
    [SerializeField] private bool isRecovering;
    [SerializeField] private bool isPunching;
    [SerializeField] private bool isSliding;
    [SerializeField] private bool isFortified;
    [SerializeField] private bool isHoldingBall;
    
    
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
    [SerializeField] private AK.Wwise.Event[] events;
    [SerializeField] RTPC engineSpeed;
    [SerializeField] RTPC vehicleSpeed;
    [SerializeField] RTPC throttle;
    [SerializeField] RTPC direction;
    [SerializeField] AK.Wwise.Switch onGround;
    [SerializeField] AK.Wwise.Switch offGround;

    
    [Header("Events")] 
    [SerializeField] public UnityEvent<GameObject> OnHitByPunch;
    [SerializeField] public UnityEvent<GameObject> OnHitbySlide;
    [SerializeField] public UnityEvent<GameObject> OnHitbByBall;
    [SerializeField] public UnityEvent<GameObject> OnGrabbingBall;
    
    
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
        OnGrabbingBall.AddListener(onGrabbingBall);
    }


    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnPlayerInstantiate(gameObject);
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
        
        //Apply float input values to engine sound effect
        throttle.SetValue(gameObject, driveInput);
        direction.SetValue(gameObject, steerInput);
        
        //Apply drag force
        currentSpeed -= currentSpeed * drag * Time.deltaTime;
        if (!isGrounded)
        {
            offGround.SetValue(gameObject);
        }
        else if (driveInput > 0f)
        {
            isBraking = false;
            isAccelerating = true;
            currentSpeed = currentSpeed + forwardAccel * Time.deltaTime;
            onGround.SetValue(gameObject);
        }
        else if(driveInput < 0f)
        {
            isBraking = true;
            isAccelerating = false;
            currentSpeed = currentSpeed * (1-brakingRatio * Time.deltaTime);
            onGround.SetValue(gameObject);
        } else
        {
            isBraking = false;
            isAccelerating = false;
            onGround.SetValue(gameObject);
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
    #endregion

    #region DRIVING_FUNCTIONS

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

    #endregion



    #region INPUT EVENT CALLBACKS

    public void onPunch(InputAction.CallbackContext context)
    {
        if (!isSliding && !isPunching && isGrounded && !isRecovering)
        {
            if (!isHoldingBall)
            {
                StartCoroutine(Punch());
            }
            else
            {
                StartCoroutine(BallPunch());
            }
        }
    }

    public void onSlide(InputAction.CallbackContext context)
    {
        if (!isSliding && !isPunching && isGrounded && !isRecovering)
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
        if (isPunching != true)
        {
            StartCoroutine(slideReactWindow());
            return;
        }
        slideCounter();
    }
    
    void onHitByPunch(GameObject source)
    {
        if (isPunching != true)
        {
            StartCoroutine(punchReactWindow());
            return;
        }
        punchCounter();
    }
    
    void onHitByBall(GameObject source)
    {
        if (isPunching != true)
        {
            StartCoroutine(ballReactWindow());
            return;
        }
        ballCounter();
    }

    void onGrabbingBall(GameObject ball)
    {
        isHoldingBall = true;
        ball.transform.SetParent(ballAnchorPoint.transform);
        ball.transform.position = ballAnchorPoint.transform.position;
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
        collider.transform.position = character.position + character.up + character.forward * 0.5f;
        yield return new WaitForSeconds(0.5f);
        Destroy(collider);
        isPunching = false;
        isRecovering = true;
        yield return new WaitForSeconds(1);
        isRecovering = false;
    }
    
    IEnumerator Slide()
    {
        isSliding = true;
        GameObject collider = Instantiate(slideCollider,character);
        ColliderBox box = collider.GetComponent<ColliderBox>();
        box.SetSource(character.gameObject);
        box.SetType(ColliderBox.ColliderType.Slide);
        collider.transform.position = character.position + character.up + character.forward * 0.5f;
        yield return new WaitForSeconds(0.5f);
        Destroy(collider);
        isSliding = false;
        isRecovering = true;
        yield return new WaitForSeconds(1);
        isRecovering = false;
    }
    
    IEnumerator BallPunch()
    {
        isPunching = true;
        GameObject collider = Instantiate(punchCollider,character);
        ColliderBox box = collider.GetComponent<ColliderBox>();
        box.SetSource(character.gameObject);
        box.SetType(ColliderBox.ColliderType.Ball);
        collider.transform.position = character.position + character.up + character.forward * 0.5f;
        yield return new WaitForSeconds(0.5f);
        Destroy(collider);
        isPunching = false;
        isRecovering = true;
        yield return new WaitForSeconds(1);
        isRecovering = false;
    }

    IEnumerator punchReactWindow()
    {
        yield return new WaitForSeconds(0.5f);
        if (isPunching != true)
        {
            punchHit();
        }
        else
        {
            punchCounter();
        }
    }
    
    IEnumerator slideReactWindow()
    {
        yield return new WaitForSeconds(0.5f);
        if (isSliding != true)
        {
            slideHit();
        }
        else
        {
            slideCounter();
        }
    }
    
    IEnumerator ballReactWindow()
    {
        yield return new WaitForSeconds(0.5f);
        if (isPunching != true)
        {
            ballHit();
        }
        else
        {
            ballCounter();
        }
    }
    
    
    
    #endregion
    
    
    #region COMBAT HITS & COUNTERS

    void punchHit()
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.green;
        particleSystem.Play();
    }

    void punchCounter()
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.white;
        particleSystem.Play();
    }

    void slideHit()
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.yellow;
        particleSystem.Play();
    }

    void slideCounter()
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.white;
        particleSystem.Play();
    }

    void ballHit()
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.red;
        particleSystem.Play();
    }

    void ballCounter()
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.magenta;
        particleSystem.Play();
    }
    
    #endregion

    #region WWISE RELATED FUNCTIONS

    public void PlayWwiseEvent(int i)
    {
        events[i].Post(gameObject);
    }

    #endregion
}

