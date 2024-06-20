using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


[Serializable]
public struct PlayerData
{
    public int nb;
    public PlayerInput input;
    public Color color;
    public bool isReady;
}

public class PlayerInstance : MonoBehaviour
{
    //[field: SerializeField] public PlayerInput playerInput { get; private set; }
    [field: SerializeField] public  PlayerData playerData { get;  set; }

    public void OnDisable()
    {
        playerData.input.actions.FindActionMap("Menu").FindAction("Join").started -= OnReady;
        playerData.input.actions.FindActionMap("Menu").FindAction("Cancel").started -= OnCancel;
        //playerData.input.actions.FindActionMap("Menu").FindAction("Rebind").started -= OnRebind;
        //playerData.input.actions.FindActionMap("Rebinding").FindAction("Rebind").started -= OnRebindExit;
    }
    
    public void Start()
    {
       playerData.input.actions.FindActionMap("Menu").FindAction("Join").started += OnReady;
       playerData.input.actions.FindActionMap("Menu").FindAction("Cancel").started += OnCancel;
       // playerData.input.actions.FindActionMap("Menu").FindAction("Rebind").started += OnRebind;
       // playerData.input.actions.FindActionMap("Rebinding").FindAction("Rebind").started += OnRebindExit;
    }
    
    public void SetPlayerColor(Color color)
    {
        var data = playerData;
        data.color = color;
        playerData = data;
    }
    
    public void OnReady(InputAction.CallbackContext context)
    {
        if(context.started)
            MenuManager.Instance.OnPlayerNext(this);
            //MenuManager.Instance.OnPlayerReady(this);
    }
    
    public void OnCancel(InputAction.CallbackContext context)
    {
        if(context.started)
            MenuManager.Instance.OnPlayerCancel(this);
            //MenuManager.Instance.CancelPlayerJoin();
    }

    public void OnRebind(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            playerData.input.SwitchCurrentActionMap("Rebinding");
            MenuManager.Instance.OnPlayerRebind(this);
        }
        //MenuManager.Instance.CancelPlayerJoin();
    }
    private void OnRebindExit(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            playerData.input.SwitchCurrentActionMap("Menu");
            MenuManager.Instance.OnPlayerRebind(this);
        }
    }

    
    
    public void ToggleReady(bool isReady)
    {
        var data = playerData;
        data.isReady = isReady;
        playerData = data;
    }
}
