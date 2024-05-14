using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController2 controller;
    public PlayerUI ui;
    public WwiseListener listener;
    public AK.Wwise.Switch[] playerID;

    private void Awake()
    {
        controller = GetComponent<PlayerController2>();
        GameManager.Instance.OnPlayerInstantiate(this);
    }

    public void setPlayerID(int nb)
    {
        playerID[nb].SetValue(this.gameObject);
        Debug.Log(playerID[nb].ToString());
    }
}
