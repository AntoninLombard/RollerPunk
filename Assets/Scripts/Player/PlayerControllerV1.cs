using System.Collections;
using UnityEngine;
using AK.Wwise;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region VARIABLES

    [SerializeField] private PlayerControllerData controllerData; 

    [field: Header("DRIVING STATE")]
    [field: SerializeField] public bool isAccelerating { get; private set; }
    [field: SerializeField] public bool isBraking { get; private set; }
    [field: SerializeField] public bool isTurning { get; private set; }
    [field: SerializeField] public bool isGrounded { get; private set; }   
    [SerializeField] private float speed = 0;
    [SerializeField] private float deltaSpeed = 0;
    [SerializeField] private Vector3 currentVelocity;
    
    [Header("CHARACTER STATE")]
    [SerializeField] private bool isRecovering;
    [SerializeField] private bool isPunching;
    [SerializeField] private bool isSliding;
    [SerializeField] private bool isStunned;
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
    
    
    [Header("Events")] 
    [SerializeField] public UnityEvent<GameObject> OnHitByPunch;
    [SerializeField] public UnityEvent<GameObject> OnHitbySlide;
    [SerializeField] public UnityEvent<GameObject> OnHitbByBallPunch;
    [SerializeField] public UnityEvent<GameObject> OnHitbByBallSlide;
    [SerializeField] public UnityEvent<GameObject> OnGrabbingBall;
    
    
    [Header("CHARACTER PARTS")]
    [SerializeField]
    public Rigidbody rb;
    [SerializeField] private Transform character;
    [SerializeField] private GameObject ballAnchorPoint;
    [SerializeField] private ParticleSystem particleSystem;

    public GameObject lastPlayerToCallWwiseEvent;

    #endregion

    #region UNITY FUNCTIONS

    private void Awake()
    {
        OnHitByPunch.AddListener(onHitByPunch);
        OnHitbySlide.AddListener(onHitBySlide);
        OnHitbByBallPunch.AddListener(onHitByBallPunch);
        OnHitbByBallSlide.AddListener(onHitByBallSlide);
        OnGrabbingBall.AddListener(onGrabbingBall);
    }


    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnPlayerInstantiate(this.gameObject);
        driveAction = input.actions.FindAction("Driving/Drive");
        steerAction = input.actions.FindAction("Driving/Steer");
        controllerData.startEngineSound.Post(this.gameObject);
    }

    
    // Update is called once per frame
    void Update()
    {
        character.position = rb.position - character.up*0.5f;
        steerInput = steerAction.ReadValue<float>();
        driveInput = driveAction.ReadValue<float>();
        
        //Apply float input values to engine sound effect
        controllerData.throttle.SetValue(gameObject, driveInput);
        controllerData.direction.SetValue(gameObject, steerInput);
        

        if (steerInput != 0 && !isStunned)
        {
            var rotation = character.rotation;
            rotation = Quaternion.Euler(Vector3.Lerp(rotation.eulerAngles, rotation.eulerAngles + new Vector3(0, steerInput * controllerData.turningRate, 0), Time.deltaTime * 5f));
            character.rotation = rotation;
            isTurning = true;
        }
        else
        {
            isTurning = false;
        }
        groundCheck(Time.deltaTime);
        controllerData.engineSpeed.SetValue(gameObject,speed);
}
    
    void FixedUpdate()
    {
        currentVelocity = rb.velocity;
        Vector3 deltaVelocity = Vector3.zero;
        
        deltaVelocity -= currentVelocity * (controllerData.drag * Time.fixedDeltaTime);
        
        
        if (!isGrounded)
        {
            controllerData.offGround.SetValue(gameObject);
        }
        else if(!isStunned)
        {
            rb.AddForce(Vector3.down * controllerData.gravityStrength, ForceMode.Acceleration);
            rb.AddForce(-character.up * controllerData.gripAccel, ForceMode.Acceleration);
            controllerData.onGround.SetValue(gameObject);
            switch (driveInput)
            {
                case > 0f:
                    isBraking = false;
                    isAccelerating = true;
                    deltaVelocity += character.forward * (driveInput * controllerData.forwardAccel * (!isHoldingBall? 1f : controllerData.ballAccelMultiplier) * Time.fixedDeltaTime);
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

        rb.AddForce(deltaVelocity ,ForceMode.VelocityChange);

        
        if (currentVelocity.magnitude > controllerData.maxSpeed * (!isHoldingBall? 1 : controllerData.ballMaxSpeedMultipier))
        {
            currentVelocity = currentVelocity.normalized * (controllerData.maxSpeed * (!isHoldingBall ? 1 : controllerData.ballMaxSpeedMultipier));
            rb.AddForce(currentVelocity - currentVelocity, ForceMode.VelocityChange);
        }
        

        deltaSpeed = deltaVelocity.magnitude;
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
            if (!isHoldingBall)
            {
                StartCoroutine(Slide());
            }
            else
            {
                StartCoroutine(BallSlide());
            }

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
    
    void onHitByBallPunch(GameObject source)
    {
        if (isPunching != true)
        {
            StartCoroutine(ballPunchReactWindow());
            return;
        }
        ballPunchCounter();
    }
    
    void onHitByBallSlide(GameObject source)
    {
        if (isPunching != true)
        {
            StartCoroutine(ballSlideReactWindow());
            return;
        }
        ballSlideCounter();
    }

    void onGrabbingBall(GameObject ball)
    {
        isHoldingBall = true;
        ball.transform.SetParent(ballAnchorPoint.transform);
        ball.transform.position = ballAnchorPoint.transform.position;
        controllerData.grabbingBallSound.Post(gameObject);
    }

    #endregion


    #region COMBAT COROUTINES

    IEnumerator Punch()
    {
        isPunching = true;
        controllerData.punchSound.Post(gameObject);
        lastPlayerToCallWwiseEvent = this.gameObject;
        GameObject hitBox = Instantiate(controllerData.punchCollider,character);
        ColliderBox box = hitBox.GetComponent<ColliderBox>();
        box.SetSource(character.gameObject);
        box.SetType(ColliderBox.ColliderType.Punch);
        hitBox.transform.position = character.position + character.up + character.forward * 0.5f;
        yield return new WaitForSeconds(0.5f);
        Destroy(hitBox);
        isPunching = false;
        isRecovering = true;
        yield return new WaitForSeconds(controllerData.actionsCooldown);
        isRecovering = false;
    }
    
    IEnumerator Slide()
    {
        isSliding = true;
        controllerData.slideSound.Post(gameObject);
        lastPlayerToCallWwiseEvent = this.gameObject;
        GameObject hitBox = Instantiate(controllerData.slideCollider,character);
        ColliderBox box = hitBox.GetComponent<ColliderBox>();
        box.SetSource(character.gameObject);
        box.SetType(ColliderBox.ColliderType.Slide);
        hitBox.transform.position = character.position + character.up + character.forward * 0.5f;
        yield return new WaitForSeconds(0.5f);
        Destroy(hitBox);
        isSliding = false;
        isRecovering = true;
        yield return new WaitForSeconds(controllerData.actionsCooldown);
        isRecovering = false;
    }
    
    
    IEnumerator BallPunch()
    {
        isPunching = true;
        controllerData.balLPunchSound.Post(gameObject);
        lastPlayerToCallWwiseEvent = this.gameObject;
        GameObject hitBox = Instantiate(controllerData.ballPunchCollider,character);
        ColliderBox box = hitBox.GetComponent<ColliderBox>();
        box.SetSource(character.gameObject);
        box.SetType(ColliderBox.ColliderType.BallPunch);
        hitBox.transform.position = character.position + character.up + character.forward * 0.5f;
        yield return new WaitForSeconds(0.5f);
        Destroy(hitBox);
        isPunching = false;
        isRecovering = true;
        yield return new WaitForSeconds(controllerData.actionsCooldown);
        isRecovering = false;
    }
    
    IEnumerator BallSlide()
    {
        isSliding = true;
        controllerData.ballSlideSound.Post(gameObject);
        lastPlayerToCallWwiseEvent = this.gameObject;
        GameObject hitBox = Instantiate(controllerData.ballSlideCollider,character);
        ColliderBox box = hitBox.GetComponent<ColliderBox>();
        box.SetSource(character.gameObject);
        box.SetType(ColliderBox.ColliderType.BallSlide);
        hitBox.transform.position = character.position + character.up + character.forward * 0.5f;
        yield return new WaitForSeconds(0.5f);
        Destroy(hitBox);
        isSliding = false;
        isRecovering = true;
        yield return new WaitForSeconds(controllerData.actionsCooldown);
        isRecovering = false;
    }

    IEnumerator punchReactWindow()
    {
        yield return new WaitForSeconds(controllerData.counterWindow);
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
        yield return new WaitForSeconds(controllerData.counterWindow);
        if (isSliding != true)
        {
            slideHit();
        }
        else
        {
            slideCounter();
        }
    }
    
    IEnumerator ballPunchReactWindow()
    {
        yield return new WaitForSeconds(controllerData.counterWindow);
        if (isPunching != true)
        {
            ballPunchHit();
        }
        else
        {
            ballPunchCounter();
        }
    }
    
    IEnumerator ballSlideReactWindow()
    {
        yield return new WaitForSeconds(controllerData.counterWindow);
        if (isPunching != true)
        {
            ballSlideHit();
        }
        else
        {
            ballSlideCounter();
        }
    }
    
    IEnumerator Stun()
    {
        isStunned = true;
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(controllerData.stunDuration);
        isStunned = false;
    }
    
    IEnumerator Death()
    {
        isStunned = true;
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(controllerData.stunDuration);
        isStunned = false;
        GameManager.Instance.OnPlayerDeath(this);
    }
    
    #endregion
    
    
    #region COMBAT HITS & COUNTERS

    void punchHit()
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.green;
        controllerData.punchHitSound.Post(gameObject);
        lastPlayerToCallWwiseEvent = this.gameObject;
        particleSystem.Play();
        StartCoroutine(Stun());
    }

    void punchCounter()
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.white;
        controllerData.punchCounterSound.Post(gameObject);
        lastPlayerToCallWwiseEvent = this.gameObject;
        particleSystem.Play();
    }

    void slideHit()
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.yellow;
        controllerData.slideHitSound.Post(gameObject);
        lastPlayerToCallWwiseEvent = this.gameObject;
        particleSystem.Play();
        StartCoroutine(Stun());
    }

    void slideCounter()
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.white;
        controllerData.slideCounterSound.Post(gameObject);
        lastPlayerToCallWwiseEvent = this.gameObject;
        particleSystem.Play();
    }

    void ballSlideHit()
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.red;
        controllerData.ballSlideHitSound.Post(gameObject);
        lastPlayerToCallWwiseEvent = this.gameObject;
        particleSystem.Play();
        StartCoroutine(Death());
    }
    
    void ballPunchHit()
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.red;
        controllerData.ballPunchHitSound.Post(gameObject);
        lastPlayerToCallWwiseEvent = this.gameObject;
        particleSystem.Play();
        StartCoroutine(Death());
    }

    void ballPunchCounter()
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.magenta;
        controllerData.ballPunchCounterSound.Post(gameObject);
        lastPlayerToCallWwiseEvent = this.gameObject;
        particleSystem.Play();
    }
    
    void ballSlideCounter()
    {
        ParticleSystem.MainModule particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.magenta;
        controllerData.ballSlideCounterSound.Post(gameObject);
        lastPlayerToCallWwiseEvent = this.gameObject;
        particleSystem.Play();
    }

    #endregion


    
    
}

