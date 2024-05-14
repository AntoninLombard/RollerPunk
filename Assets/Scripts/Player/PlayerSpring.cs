using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpring : MonoBehaviour
{
    [SerializeField] private PlayerController2 playerController;

    [SerializeField] private Transform character;
    [SerializeField] private Transform upperBody;
    //[SerializeField] private Transform upperBone;
    //[SerializeField] private Transform lowerBone;
    [SerializeField] private Transform wheel;
    [SerializeField] private float wheelRadius;
    [SerializeField] private LayerMask physicMask;
    private Vector3 delta;
    private Vector3 axis;
    
    [Header("Spring Physic Variables")]
    [SerializeField] private float currentLength;
    [SerializeField] private float bodyWeight;
    [SerializeField] private float downForce;
    [SerializeField] [Range(0.0f,1.0f)] private float damping;
    [SerializeField] [Range(0.0f,10.0f)]private float springForce;
    [SerializeField] private float restingLength;
    [SerializeField] private float velocity;
    
    // Start is called before the first frame update
    void Start()
    {
        delta = wheel.position- upperBody.position;
        axis = character.up;
        currentLength = delta.magnitude;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        placingWheel(axis,Time.fixedDeltaTime);
        axis = -character.up;
        delta = wheel.position- upperBody.position;
        currentLength = delta.magnitude;
        
        
        velocity *= damping;
        velocity -= (currentLength - restingLength) * springForce;
        // if (playerController.isGrounded)
        // {
        //     velocity += bodyWeight * downForce * Time.deltaTime;
        // }
        currentLength += velocity * Time.fixedDeltaTime;
        upperBody.position -= axis * (velocity * Time.fixedDeltaTime) ;


    }
    

    void placingWheel(Vector3 direction,float time)
    {
        if (Physics.Raycast(character.position, direction, out var hit, 2f*restingLength,physicMask))
        {
            wheel.position = hit.point - axis * wheelRadius;
        }
        else
        {
            wheel.position = Vector3.Lerp(wheel.position, wheel.position + direction * restingLength,time * 2f);
        }
    }

    private void OnDrawGizmos()
    {
        if (Physics.Raycast(character.position, axis, restingLength * 2,physicMask))
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawLine(character.position,character.position + axis * restingLength*2);
    }
}
