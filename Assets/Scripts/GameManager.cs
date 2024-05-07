using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;
using JetBrains.Annotations;
using Random = UnityEngine.Random;
using Unity.VisualScripting;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    
    [Header("Players")]
    [SerializeField] private Dictionary<Player,int> players = new Dictionary<Player, int>();
    [SerializeField] private int nbPlayer;

    [Header("Gameplay")] 
    [SerializeField] private bool isRaceOn;
    [SerializeField] private float timer;
    [SerializeField] private List<Checkpoint> checkpoints;
    [SerializeField] private List<Transform> spawnPoints;
    
    [Header("Ball & Ball holder")]
    [SerializeField] private BallScript ball;
    [SerializeField] [CanBeNull] private Player ballHolder;
    [SerializeField] private Vector3 holderPreviousPosition;
    [SerializeField] private float ballDistance = 0;

    [Header("Wwise")]
    [SerializeField] private AK.Wwise.Switch[] playerNumber;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isRaceOn)
        {
            timer -= Time.deltaTime;
            if (timer<0)
            {
                timer = 0;
            }
        }
    }


    void StartRace()
    {
        isRaceOn = true;
    }

    public void OnPlayerInstantiate(Player player)
    {
        players.Add(player, 0);
        //player.controller.rb.position = spawnPoints[nbPlayer].position; //for V1
        player.controller.character.position = spawnPoints[nbPlayer].position; //for V2
        playerNumber[nbPlayer].SetValue(player.gameObject);
        Debug.Log(playerNumber[nbPlayer].ToString());
        nbPlayer++;
        player.listener.SetVolumeOffset(nbPlayer);
    }
    public void OnPlayerDeath(Player player)
    {
        Vector3 pos = spawnPoints[Random.Range(0, nbPlayer)].position;
        //player.transform.position = pos;
        //player.controller.rb.position = pos; //for V1
        player.controller.character.position = pos; //for V2

    }

    public void OnScoreChange(Player player, int points)
    {
        players[player] += points;
        player.GetComponent<PlayerUI>()?.OnScoreChange(players[player]);
    }

    public void OnBallGrabbed(Player player)
    {
        ballHolder = player;
        
    }
    
    public void OnPointScored(Player player)
    {
        //players[player] += points;
        //player.GetComponent<PlayerUI>()?.OnScoreChange(players[player]);
    }

    public void UpdateBallDistance()
    {
        if (ballHolder != null)
        {
            //ballDistance += 
        }
    }
}
