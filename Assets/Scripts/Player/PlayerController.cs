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
    [SerializeField] private float brakingAccel = 0;
    [SerializeField] private float turningRate = 0;
    [SerializeField] private float gravityStrength = 0.0f;
    
    [SerializeField] private bool isAccelerating = false;
    [SerializeField] private bool isBraking = false;
    [SerializeField] private bool isTurning = false;

    [SerializeField] private float acceleration = 0;
    [SerializeField] private float turningValue = 0;


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

        if (Input.GetAxis("Vertical") > 0f)
        {
            isBraking = false;
            isAccelerating = true;
        }
        else if(Input.GetAxis("Vertical") < 0f)
        {
            isBraking = true;
            isAccelerating = false;
        } else
        {
            isBraking = false;
            isAccelerating = false;
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
            isTurning = true;
            turningValue = Input.GetAxis("Horizontal");
        }
        else
        {
            isTurning = false;
        }
    }
    
    void FixedUpdate()
    {
        if (isAccelerating & !isBraking)
        {
            rb.AddForce(transform.forward * forwardAccel,ForceMode.Acceleration);
        }

        if (isBraking && rb.velocity.magnitude > 0)
        {
            rb.AddForce(-transform.forward * (brakingAccel * rb.velocity.magnitude),ForceMode.Acceleration);
        }

        if (isTurning)
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0,transform.eulerAngles.y + turningValue * turningRate, 0), Time.deltaTime * 5f);
        }
        
        
        RaycastHit hitOn;
        Physics.Raycast(character.position + (character.up * .1f), Vector3.down, out hitOn);
        character.up = Vector3.Lerp(character.up, hitOn.normal, Time.deltaTime * 8.0f);
        //rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
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

