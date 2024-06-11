using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputHandler : MonoBehaviour
{
   [SerializeField] private List<PlayerInput> playerInputs;
   
   private void Awake()
   {
      SceneManager.sceneLoaded += OnSceneLoaded;
      DontDestroyOnLoad(this);
   }

   public void OnPlayerJoin(PlayerInput playerInput)
   {
      DontDestroyOnLoad(playerInput);
      playerInputs.Add(playerInput);
      playerInput.gameObject.transform.parent = transform;
      //MenuManager.Instance.OnPlayerJoin(playerInput);
   }

   void OnSceneLoaded(Scene scene, LoadSceneMode mode)
   {
      if (scene.name == "UITest")
         return;
      foreach (var playerInput in playerInputs)
      {
         GameManager.Instance.SpawnPlayer(playerInput);
      }

      GameManager.Instance.SplitScreenCamera();
   }
}
