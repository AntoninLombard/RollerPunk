using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public PlayerController2 controller;
    public PlayerUI ui;
    public PlayerCombat combat;
    public PlayerInput input;
    public Transform character;
    public PlayerControllerData data;


    private void Awake()
    {
        //controller = GetComponent<PlayerController2>();
        GameManager.Instance.OnPlayerInstantiate(this);
        Debug.Log("Test");
    }

    private void Update()
    {
        Debug.Log("Test");
    }

    private void OnDestroy()
    {
        Debug.Log("DestroyingPlayer");
    }
}
