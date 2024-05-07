using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController2 controller;
    public PlayerUI ui;


    private void Awake()
    {
        controller = GetComponent<PlayerController2>();
        GameManager.Instance.OnPlayerInstantiate(this);
    }
}
