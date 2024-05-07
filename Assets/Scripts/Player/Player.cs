using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController controller;
    public PlayerUI ui;


    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        GameManager.Instance.OnPlayerInstantiate(this);
    }
}
