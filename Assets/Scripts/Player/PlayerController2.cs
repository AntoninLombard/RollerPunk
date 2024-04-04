using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using AK.Wwise;

public class PlayerController2 : MonoBehaviour
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

    [SerializeField] private Vector3 velocity;


    [SerializeField] private Transform character;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerInput input;

    private InputAction driveAction;
    private InputAction steerAction;

    [Header("Sound Variables")]
    [SerializeField] private AK.Wwise.RTPC engineSpeed;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        driveAction = input.actions.FindAction("Player/Drive");
        steerAction = input.actions.FindAction("Player/Steer");
    }

    // Update is called once per frame
    void Update()
    {
        velocity = Vector3.zero;
        move(Time.deltaTime);
        steer(Time.deltaTime);
        gravity(Time.deltaTime);
        groundCheck(Time.deltaTime);
        engineSpeed.SetValue(gameObject, currentSpeed);
    }

    private void FixedUpdate()
    {
        //rb.velocity = velocity;
        rb.AddForce(velocity,ForceMode.Acceleration);
    }


    #region MY FUNCTIONS

    private void move(float time)
    {
        float driveInput = driveAction.ReadValue<float>();
        currentSpeed = currentSpeed * (1f-drag*time);
        if (!isGrounded)
        {
        }
        if (driveInput > 0)
        {
            currentSpeed += forwardAccel * time;
        } else 
        {
            currentSpeed = currentSpeed * (1f-brakingRatio*time);
        }

        if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }
        else if (currentSpeed < 0.001)
        {
            currentSpeed = 0;
        }

        velocity += character.forward * currentSpeed;
    }

    private void steer(float time)
    {
        float steerInput = steerAction.ReadValue<float>();
        turningInput = steerInput;
        if (steerInput != 0f)
        {
            character.rotation = Quaternion.Euler(Vector3.Lerp(character.rotation.eulerAngles, character.rotation.eulerAngles + new Vector3(0, steerInput * turningRate, 0), time * 5f));
        }
    }

    private void gravity(float time)
    {
        velocity += Vector3.down * gravityStrength;
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
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        RaycastHit hit;
        if (Physics.Raycast(character.position, -character.up,out hit,0.5f))
        {
            Gizmos.color = Color.green;
        }
        //Draw the suspension
        Gizmos.DrawLine(character.position, character.position + -character.up * 0.5f);
    }
    
    #endregion
}
