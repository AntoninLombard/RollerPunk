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
    public Camera camera;
    public Transform character;
    public PlayerControllerData data;
    public Animator anime;
	public WwiseListener listener;
    public AK.Wwise.Switch[] playerID;
    
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

    public void setPlayerID(int nb)
    {
        playerID[nb].SetValue(this.gameObject);
        Debug.Log(playerID[nb].ToString());
    }
}
