using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] public PlayerInput playerInput;
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
        // driveAction = playerInput.actions.FindAction("Driving/Drive");
        // brakeAction = playerInput.actions.FindAction("Driving/Reverse");
        // steerAction = playerInput.actions.FindAction("Driving/Steer");
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

    public void onPunchL(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            punchInput = true;
            player.combat.StartPunch(-1);
        }
        else if(context.canceled)
        {
            punchInput = false;
            player.combat.CancelPunch();
        }
    }
    
    public void onPunchR(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            punchInput = true;
            player.combat.StartPunch(1);
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
    
    public void onPause(InputAction.CallbackContext context)
    {
        GameManager.Instance.TogglePause(true);
    }

    #endregion


    public void SetInput(PlayerInput playerInput)
    {
        this.playerInput = playerInput;
        playerInput.currentActionMap = playerInput.actions.FindActionMap("Driving");
        playerInput.actions.FindActionMap("Menu").Disable();
        driveAction = playerInput.actions.FindAction("Driving/Drive");
        brakeAction = playerInput.actions.FindAction("Driving/Reverse");
        steerAction = playerInput.actions.FindAction("Driving/Steer");

        playerInput.actions.FindAction("Driving/PunchL").started += onPunchL;
        playerInput.actions.FindAction("Driving/PunchL").performed += onPunchL;
        playerInput.actions.FindAction("Driving/PunchL").canceled += onPunchL;
        
        playerInput.actions.FindAction("Driving/PunchR").started += onPunchR;
        playerInput.actions.FindAction("Driving/PunchR").performed += onPunchR;
        playerInput.actions.FindAction("Driving/PunchR").canceled += onPunchR;
        
        playerInput.actions.FindAction("Driving/Parry").started += onParry;
        playerInput.actions.FindAction("Driving/Parry").performed += onParry;
        playerInput.actions.FindAction("Driving/Parry").canceled += onParry;
        
        playerInput.actions.FindAction("Driving/Drift").started += onDrift;
        playerInput.actions.FindAction("Driving/Drift").started += onDrift;
        playerInput.actions.FindAction("Driving/Drift").canceled += onDrift;
        
        playerInput.actions.FindAction("Driving/Pause").started += onPause;

    }
}
