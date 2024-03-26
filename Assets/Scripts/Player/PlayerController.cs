using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region VARIABLES
    
    [Header("DRIVING VALUES")]
    [SerializeField] private float forwardAccel = 0;
    [SerializeField] private float brakingRatio = 0;
    [SerializeField] private float turningRate = 0;
    [SerializeField] private float gravityStrength = 0.0f;
    
    
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
    
    
    
    
    
    [SerializeField] private Rigidbody rb;
    
    
    #region UNITY FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    // Update is called once per frame
    void Update()
    {
        transform.position = rb.position;

        turningInput = Input.GetAxis("Horizontal");
        accelInput = Input.GetAxis("Vertical");
        
        
        if (accelInput > 0f)
        {
            isBraking = false;
            isAccelerating = true;
            currentSpeed = currentSpeed + forwardAccel * Time.deltaTime;
        }
        else if(accelInput < 0f)
        {
            isBraking = true;
            isAccelerating = false;
            currentSpeed = currentSpeed * brakingRatio * Time.deltaTime;
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
    }
    
    void FixedUpdate()
    {
        
        rb.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
        
        
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
            isGrounded = true;
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0,transform.eulerAngles.y + turningInput * turningRate, 0), Time.deltaTime * 5f);
        }
        
        //Applying gravity force down ward
        //rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
        
        //Adjust character model to the surface normal it's on
        RaycastHit hitOn;
        if (Physics.Raycast(character.position, Vector3.down * 0.5f, out hitOn))
        {
            isGrounded = true;
            character.up = Vector3.Lerp(character.up, hitOn.normal, Time.deltaTime * 8.0f);
            character.Rotate(0, transform.eulerAngles.y, 0);
        }
        else
        {
            isGrounded = false;
        }

        speed = rb.velocity.magnitude;
    }
    
    #endregion
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Draw the suspension
        Gizmos.DrawLine(
            transform.position + Vector3.up *3f, transform.position + rb.velocity * 10f + Vector3.up *3f
        );
    }
}

