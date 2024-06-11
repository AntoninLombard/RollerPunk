using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInstance : MonoBehaviour
{
    [field: SerializeField] public PlayerInput playerInput { get; private set; }



    public void OnReady()
    {
        MenuManager.Instance.OnPlayerReady(playerInput);
    }
}
