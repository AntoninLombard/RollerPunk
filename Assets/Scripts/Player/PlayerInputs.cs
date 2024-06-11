using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerInputData inputData;
    
    [field: Header("PLAYER DRIVING INPUTS")]
    [field: SerializeField] [field: Range(-1.0f,1.0f)] public float steerInput { get; private set; }
    [field: SerializeField] [field: Range(0f,1.0f)] public float driveInput { get; private set; }
    [field: SerializeField] [field: Range(0f,1.0f)] public float brakeInput { get; private set; }
    [SerializeField] public int lastSteerSide;
    
    
    [Header("PLAYER COMBAT INPUTS")] 
    [SerializeField] private bool punchInput;
    [SerializeField] private bool slideInput;
    [SerializeField] private bool fortifyInput;
    [SerializeField] private bool driftInput;
    
    
    private InputAction driveAction;
    private InputAction brakeAction;
    private InputAction steerAction;

    
    void Start()
    {
        driveAction = playerInput.actions.FindAction("Driving/Drive");
        brakeAction = playerInput.actions.FindAction("Driving/Reverse");
        steerAction = playerInput.actions.FindAction("Driving/Steer");
    }
    private void Update()
    {
        steerInput = inputData.steerCurve.Evaluate(steerAction.ReadValue<float>());
        driveInput = inputData.driveCurve.Evaluate(driveAction.ReadValue<float>());
        brakeInput = inputData.brakeCurve.Evaluate(brakeAction.ReadValue<float>());
        if(steerInput != 0)
            lastSteerSide = (steerInput > 0)? 1 : -1;
    }

    #region INPUT EVENT CALLBACKS

    public void onPunch(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            punchInput = true;
            player.combat.StartPunch();
        }
        else if(context.canceled)
        {
            punchInput = false;
            player.combat.CancelPunch();
        }
    }

 
    public void onParry(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            fortifyInput = true;
            player.combat.StartParry();
        } else if (context.canceled)
        {
            fortifyInput = false;
        }
    }
    
    public void onDrift(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            driftInput = true;
            player.controller.StartDrift();
        }
        else if(context.canceled)
        {
            driftInput = false;
            player.controller.CancelDrift();
        }
    }
    
    public void onReady(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            player.isReady = true;
            player.ui.SetReady(true);
            GameManager.Instance.OnPlayerReady();
            Debug.Log("StartPressed");
        } 

    }

    #endregion
    
    
}
