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

    public void Start()
    {
       playerData.input.actions.FindActionMap("Menu").FindAction("Join").started += OnReady;
       playerData.input.actions.FindActionMap("Menu").FindAction("Cancel").started += OnCancel;
    }

    public void SetPlayerColor(Color color)
    {
        var data = playerData;
        data.color = color;
        playerData = data;
    }
    
    public void OnReady(InputAction.CallbackContext context)
    {
        MenuManager.Instance.OnPlayerReady(this);
    }
    
    public void OnCancel(InputAction.CallbackContext context)
    {
        if(context.started)
            MenuManager.Instance.CancelPlayerJoin();
    }

    public void ToggleReady(bool isReady)
    {
        var data = playerData;
        data.isReady = isReady;
        playerData = data;
    }
}
