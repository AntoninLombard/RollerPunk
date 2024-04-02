using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using AK.Wwise;

public class PlayerController : MonoBehaviour
{
    #region VARIABLES

    [Header("DRIVING VALUES")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float forwardAccel = 0;
    [SerializeField] private float brakingRatio = 0;
    [SerializeField] private float turningRate = 0;
    [SerializeField] private float gravityStrength = 0.0f;
    [SerializeField] private float drag;
    
    
    [Header("DRIVING STATE (DO NOT MODIFY)")]
    [SerializeField] private bool isAccelerating = false;
    [SerializeField] private bool isBraking = false;
    [SerializeField] private bool isTurning = false;
    [SerializeField] private bool isGrounded = false;

    [SerializeField] private float speed = 0;
    [SerializeField] private float currentSpeed = 0;
    [SerializeField] private float turningInput = 0;
    [SerializeField] private float accelInput = 0;


    [SerializeField] private Transform character;
    #endregion
    
    public AK.Wwise.RTPC engineSpeed;
    
    
    
    [SerializeField] private Rigidbody rb;
    
    
    #region UNITY FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        //rb.transform.parent = null;
    }

    
    // Update is called once per frame
    void Update()
    {
        Vector3 _previousPos = rb.transform.position;
        character.position = rb.position - character.up*0.5f;
        //rb.transform.position = _previousPos;

        turningInput = Input.GetAxis("Horizontal");
        accelInput = Input.GetAxis("Vertical");
        
        
        //Apply drag force
        currentSpeed = currentSpeed * (1-drag * Time.deltaTime);
        if (!isGrounded)
        {
            
        }
        else if (accelInput > 0f)
        {
            isBraking = false;
            isAccelerating = true;
            currentSpeed = currentSpeed + forwardAccel * Time.deltaTime;
        }
        else if(accelInput < 0f)
        {
            isBraking = true;
            isAccelerating = false;
            currentSpeed = currentSpeed * (1-brakingRatio * Time.deltaTime);
        } else
        {
            isBraking = false;
            isAccelerating = false;
        }

        if (turningInput != 0)
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

        engineSpeed.SetValue(gameObject, currentSpeed);

    }
    
    void FixedUpdate()
    {
        
        rb.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
        //rb.velocity = transform.forward * currentSpeed + Vector3.down * gravityStrength;

        
        /*//Speeding up the player
        if (isAccelerating & !isBraking)
        {
            rb.AddForce(transform.forward * (forwardAccel * accelInput), ForceMode.Acceleration);
        }

        
        //Slowing down the player when braking
        if (isBraking && rb.velocity.magnitude > 0)
        {
            rb.AddForce(transform.forward * (brakingAccel * rb.velocity.magnitude * accelInput));
        }*/

        
        
        //Turning the player toward the new direction
        if (isTurning)
        {
            character.eulerAngles = Vector3.Lerp(character.eulerAngles, new Vector3(0,character.eulerAngles.y + turningInput * turningRate, 0), Time.deltaTime * 5f);
        }


        //groundCheck();
        
        //Applying gravity force down ward
        if (!isGrounded)
        {
            //rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
        }

        speed = rb.velocity.magnitude;
    }
    
    #endregion
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


    void groundCheck(float time)
    {
        //Adjust character model to the surface normal it's on
        if (Physics.Raycast(character.position, Vector3.down, out var hit, 0.5f))
        {
            isGrounded = true;
            Vector3 rot = character.rotation.eulerAngles;
            character.up = Vector3.Lerp(character.up, hit.normal, Time.deltaTime * 2.0f);
            character.Rotate(0, rot.y, 0);
        }
        else
        {
            isGrounded = false;
        }
    }
    
}

