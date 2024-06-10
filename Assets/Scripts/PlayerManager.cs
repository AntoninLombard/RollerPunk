using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [Serializable] public struct PlayerData
    {
        public PlayerInput playerInput;
        public bool isReady;
        public Color playerColor;
    }
    
    
    
    [SerializeField] private PlayerInput globalInput;
    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private List<PlayerData> players;
    
    
    // Start is called before the first frame update
    void Start()
    {
    }

    void OnPlayerInstantiate()
    {

    }
}




