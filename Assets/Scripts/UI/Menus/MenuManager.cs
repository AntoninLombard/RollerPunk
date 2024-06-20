using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
    
    [SerializeField] private PlayerInputManager inputManager;
    [SerializeField] private int minNbPlayer;
    [SerializeField] private int maxNbPlayer;
    [SerializeField] private int playerNumber;
    [SerializeField] private List<PlayerInstance> players;
    [SerializeField] public Dictionary<PlayerInstance,PlayerPanel> panelBindings;

    
    [SerializeField] private UI_Animator mainMenuPanel;
    [SerializeField] private GameObject defaultButton;
    [field : SerializeField] private PlayerPanel[] playerPanels = new PlayerPanel[4];
    
    [SerializeField] private InputSystemUIInputModule uiModule;
    [SerializeField] private MultiplayerEventSystem multyEvent;
    
    
    // private void Awake()
    // {
    // }

    // Start is called before the first frame update
    void Start()
    {
        panelBindings = new Dictionary<PlayerInstance, PlayerPanel>();
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
        uiModule.enabled = !isActive;
        //multyEvent.enabled = !isActive;
        if (isActive)
            inputManager.EnableJoining();
        else
        {
            inputManager.DisableJoining();
            PlayerInputHandler.Instance.ClearPlayers();
        }
    }


    public void OnPlayerJoin(PlayerInput input)
    {
        PlayerPanel panel = playerPanels.Except(panelBindings.Values).First();
        PlayerInstance player = input.GetComponent<PlayerInstance>();
        players.Add(player);
        PlayerData data = player.playerData;
        data.nb = playerNumber;
        data.color = gameData.playerColors[playerNumber];
        data.isReady = false;
        player.playerData = data;
        
        panelBindings.Add(player,panel);
        panel.ChangeState(PlayerPanel.PanelState.Joined);
        panel.SetColor(player.playerData.color);
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
        PlayerPanel panel = panelBindings[player];
        panel.ChangeState(PlayerPanel.PanelState.Hide);
        panelBindings.Remove(player);
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
        mainMenuPanel.gameObject.SetActive(true);
        mainMenuPanel.UIFadeIn();
        multyEvent.SetSelectedGameObject(defaultButton);
    }

    public void OnPlayerReady(PlayerInstance player)
    {
        player.ToggleReady(!player.playerData.isReady);
        PlayerPanel panel = panelBindings[player];
        // if (player.playerData.isReady)
        // {
        //     panel.ChangePrompt(PlayerPanel.PromptState.Waiting);
        // }
        // else
        // {
        //     panel.ChangePrompt(PlayerPanel.PromptState.Ready);
        // }
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
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }

    public void OnPlayerNext(PlayerInstance player)
    {
        panelBindings[player].NextState();
    }
    
    public void OnPlayerCancel(PlayerInstance player)
    {
        panelBindings[player].PreviousState();
    }


    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnPlayerRebind(PlayerInstance player)
    {
        panelBindings[player].ToggleRebindMenu();
    }
}
