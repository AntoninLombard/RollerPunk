using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpring : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [SerializeField] private Transform character;
    [SerializeField] private Transform upperBody;
    //[SerializeField] private Transform upperBone;
    //[SerializeField] private Transform lowerBone;
    [SerializeField] private Transform wheel;
    [SerializeField] private float wheelRadius;
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
        Vector3 delta = upperBody.position - wheel.position;
        axis = delta.normalized;
        currentLength = delta.magnitude;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        placingWheel(axis,Time.fixedDeltaTime);
        Vector3 detla = upperBody.position - wheel.position;
        currentLength = detla.magnitude;
        
        
        velocity -= velocity*damping;
        velocity += (currentLength - restingLength) * springForce;
        if (playerController.isGrounded)
        {
            velocity += bodyWeight * downForce * Time.deltaTime;
        }
        currentLength += velocity * Time.fixedDeltaTime;
        upperBody.position = wheel.position - axis * currentLength;


    }


    void placingWheel(Vector3 direction,float time)
    {
        if (Physics.Raycast(character.position, direction, out var hit, 0.5f))
        {
            wheel.position = hit.point + character.up * wheelRadius;
        }
        else
        {
            wheel.position = Vector3.Lerp(wheel.position, wheel.position + direction * (currentLength - restingLength),time * 2f);
        }
    }
}
