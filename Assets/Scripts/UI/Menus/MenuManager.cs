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
    [Serializable]
    private struct UIPlayerBinding
    {
        public PlayerPanel panel;
        public PlayerInstance player;
    }
    
    static public MenuManager Instance;
    
    [SerializeField] private GameData gameData;
    [SerializeField] private int trackNb;

    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private PlayerInputManager inputManager;
    [SerializeField] private int minNbPlayer;
    [SerializeField] private int maxNbPlayer;
    [SerializeField] private int playerNumber;
    [SerializeField] private List<PlayerInstance> players;
    [SerializeField] private List<UIPlayerBinding> panelBindings;

    
    [SerializeField] private UI_Animator mainMenuPanel;
    [field : SerializeField] private PlayerPanel[] playerPanels = new PlayerPanel[4];
    
    [SerializeField] private InputSystemUIInputModule uiModule;
    [SerializeField] private MultiplayerEventSystem multyEvent;
    
    
    // private void Awake()
    // {
    // }

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
        {
            inputManager.DisableJoining();
            inputHandler.ClearPlayers();
        }
    }


    public void OnPlayerJoin(PlayerInput input)
    {
        PlayerPanel playerPanel = null;
        int i = 0;
        for(;i < 4;i++)
        {
            if (panelBindings.Exists(y => y.panel == playerPanels[i])) continue;
            playerPanel = playerPanels[i];
            break;
        }

        PlayerInstance player = input.GetComponent<PlayerInstance>();
        players.Add(player);
        PlayerData data = player.playerData;
        data.nb = i;
        data.color = gameData.playerColors[i];
        data.isReady = false;
        player.playerData = data;
        
        UIPlayerBinding binding = new UIPlayerBinding();
        binding.player = player;
        binding.panel = playerPanel;
        panelBindings.Add(binding);
        binding.panel.ChangeState(PlayerPanel.PanelState.Joined);
        playerNumber++;
    }

    public void OnPlayerLeave(PlayerInput playerInput)
    {
        players.Remove(playerInput.GetComponent<PlayerInstance>());
    }

    public void OnPlayerCancel(PlayerInput playerInput)
    {
        PlayerInstance player = playerInput.GetComponent<PlayerInstance>();
        players.Remove(player);
        UIPlayerBinding binding = panelBindings.Find(x => x.player == player);
        binding.panel.ChangeState(PlayerPanel.PanelState.Hide);
        panelBindings.Remove(binding);
        if (players.Count == 0)
        {
            CancelPlayerJoin();
        }
    }
    
    
    public void CancelPlayerJoin()
    {
        PlayerJoinToggle(false);
        for(int i =0 ; i < 4;i++)
        {
            playerPanels[i].ChangeState(PlayerPanel.PanelState.Hide);
        }
        players.Clear();
        panelBindings.Clear();
        playerNumber = 0;
        mainMenuPanel.UIFadeIn();
    }

    public void OnPlayerReady(PlayerInstance player)
    {
        player.ToggleReady(!player.playerData.isReady);
        if (playerNumber < minNbPlayer)
            return;
        int count = 0;
        foreach (PlayerInstance currentPlayer in players)
        {
            if (currentPlayer.playerData.isReady)
            {
                count++;
            }
        }

        if (count != 0 && count == players.Count)
        {
            AkSoundEngine.SetState("Menu_Music", "None");
            SceneManager.LoadScene(gameData.tracks[trackNb].name, LoadSceneMode.Single);
        }
    }
}
