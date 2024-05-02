using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    
    [Header("Players")]
    [SerializeField] private List<PlayerController> players;
    [SerializeField] private int[] score;
    [SerializeField] private int nbHumanPlayer;
    [SerializeField] private int nbPlayer;

    [Header("Gameplay")] 
    [SerializeField] private bool isRaceOn;
    [SerializeField] private PlayerController ballHolder;
    [SerializeField] private float timer;
    [SerializeField] private BallScript ball;
    [SerializeField] private List<CheckpointScript> checkpoints;
    [SerializeField] private List<Transform> spawnPoints;

    public RTPC playerNumber;
    
    
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

    public void OnPlayerInstantiate(GameObject player)
    {
        players.Add(player.GetComponent<PlayerController>());
        player.transform.position =  spawnPoints[nbHumanPlayer].position;
        nbHumanPlayer++;
        nbPlayer++;
        playerNumber.SetValue(player, nbPlayer);
    }

    public void OnPlayerDeath(PlayerController player)
    {
        Vector3 pos = spawnPoints[Random.Range(0, nbHumanPlayer)].position;
        //player.transform.position = pos;
        player.rb.position = pos;

    }
}
