using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    static public MenuManager Instance;

    [SerializeField] private GameData gameData;
    [SerializeField] private int trackNb;
    
    [SerializeField] private PlayerInputManager inputManager;
    [SerializeField] private int minNbPlayer;
    [SerializeField] private int maxNbPlayer;
    [SerializeField] private int playerNumber;
    [SerializeField] private Dictionary<PlayerInput,bool> players;

    [SerializeField] private List<UI_Animator> playerPanels;
    
    [SerializeField] private InputSystemUIInputModule uiModule;
    [SerializeField] private MultiplayerEventSystem multyEvent;
    private void Awake()
    {
        players = new Dictionary<PlayerInput, bool>();
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerJoinToggle(false);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerJoinToggle(bool isActive)
    {
        multyEvent.enabled = !isActive;
        uiModule.enabled = !isActive;
        if (isActive)
            inputManager.EnableJoining();
        else
            inputManager.DisableJoining();
    }


    public void OnPlayerJoin(PlayerInput playerInput)
    {
        players.Add(playerInput,false);
        playerPanels[playerNumber].UIPanelMove(playerNumber < 2 ? 2 : 5);
        playerNumber++;
    }


    public void OnPlayerReady(PlayerInput playerInput)
    {
        players[playerInput] = !players[playerInput];
        if (playerNumber < minNbPlayer)
            return;
        int count = 0;
        foreach (KeyValuePair<PlayerInput,bool> player in players)
        {
            if (player.Value)
            {
                count++;
            }
        }

        if (count == players.Count)
        {
            AkSoundEngine.SetState("Menu_Music", "None");
            SceneManager.LoadScene(gameData.tracks[trackNb].name,LoadSceneMode.Single);
        }
    }
}
