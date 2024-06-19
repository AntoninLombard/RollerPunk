using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputHandler : MonoBehaviour
{
   public static  PlayerInputHandler Instance;
   
   [SerializeField] private List<PlayerInstance> players;
   [SerializeField] private AK.Wwise.Event backSound;
   
   ~PlayerInputHandler()
    
   {
      //Instance = null;
      SceneManager.sceneLoaded -= OnSceneLoaded;
   }
   

   private void OnEnable()
   {
      if (Instance == null)
      {
         Instance = this;
      }
      else
         Destroy(this);
      SceneManager.sceneLoaded += OnSceneLoaded;
      DontDestroyOnLoad(this);
   }

   public void OnPlayerJoin(PlayerInput playerInput)
   {
      DontDestroyOnLoad(playerInput.gameObject);
      players.Add(playerInput.GetComponent<PlayerInstance>());
      playerInput.gameObject.transform.parent = transform;
   }
   
   public void OnPlayerLeave(PlayerInput playerInput)
   {
      backSound.Post(gameObject);
      players.Remove(playerInput.GetComponent<PlayerInstance>());
   }

   public void ClearPlayers()
   {
      while(transform.childCount > 0)
      {
         Transform child = transform.GetChild(0);
         #if UNITY_EDITOR
         DestroyImmediate(child.gameObject);
         #else
         Destroy(child.gameObject);
         #endif
      }
      players.Clear();
   }

   void OnSceneLoaded(Scene scene, LoadSceneMode mode)
   {
      if (scene.name == "MainMenu")
      {
         return;
      }
      foreach (var player in players)
      {
         GameManager.Instance.SpawnPlayer(player.playerData);
      }

      GameManager.Instance.SplitScreenCamera();
   }
}
