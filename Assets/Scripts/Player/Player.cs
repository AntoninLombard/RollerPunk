using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public PlayerController2 controller;
    public PlayerUI ui;
    public PlayerCombat combat;
    public PlayerInputs input;
    public Camera camera;
    public CinemachineVirtualCamera virtualCamera;
    public Transform character;
    public PlayerControllerData data;
    public PlayerAnimation anime;
    public Color color;
    public int number;
    public bool isReady;
    
    
	public WwiseListener listener;
    public AK.Wwise.Switch[] playerID;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    public void Init()
    {
        ToggleActive(false);
        ui.ToggleCountdown(false);
        isReady = false;
        foreach (Material material in anime.animator.gameObject.GetComponentInChildren<SkinnedMeshRenderer>()?.materials)
        {
            material.SetColor(EmissionColor,color);
        } 
        anime.SetColor(color);
        ui.SetColor(color);
    }

    public void setPlayerID(int nb)
    {
        playerID[nb].SetValue(gameObject);
    }

    public void ToggleActive(bool isActive)
    {
        controller?.TogglePlayerFreeze(!isActive);
        combat?.ToggleInvincibility(!isActive);
    }
}
    
