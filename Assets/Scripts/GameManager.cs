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
    
    [field: Header("Ball & Ball holder")]
    [field: SerializeField] public BallScript ball { get; private set; }
    [field: SerializeField] [CanBeNull] public Player ballHolder { get; private set; }
    [SerializeField] private Vector3 holderPreviousPosition;
    [SerializeField] private Vector3 holderPreviousForward;
    [SerializeField] private float distanceTraveled = 0;
    [SerializeField] private int cumulatedPoints;
    [SerializeField] private int kills;
    [SerializeField] [CanBeNull] private Checkpoint lastCheckpoint;

    [Header("Audio")]
    [SerializeField] private RTPC crowds;

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

    private void FixedUpdate()
    {
        UpdateBallDistance();
    }

    void StartRace()
    {
        isRaceOn = true;
    }

    public void OnPlayerInstantiate(Player player)
    {
        players.Add(player,0);
        int layer = (int)Math.Log(player.data.playerLayer[nbPlayer], 2);
        player.camera.gameObject.layer = layer;
        foreach(LayerMask mask in player.data.playerLayer.FindAll(x => x != player.data.playerLayer[nbPlayer]))
            player.camera.cullingMask -= player.camera.cullingMask & mask;
        if (player.character != null) player.character.position = spawnPoints[nbPlayer].position;
        player.setPlayerID(nbPlayer);
        nbPlayer++;
        player.listener.SetVolumeOffset(nbPlayer);
    }
    public void OnPlayerDeath(Player player)
    {
        Vector3 pos = spawnPoints[Random.Range(0, nbPlayer)].position;
        //player.transform.position = pos;
        //player.controller.rb.position = pos; //for V1
        player.character.position = pos; //for V2

    }

    public void OnScoring()
    {
        players[ballHolder] += cumulatedPoints * (1+kills);
        ballHolder.ui.OnScoreChange(players[ballHolder]);
        ResetBall();
    }
    
    public void OnBallKill()
    {
        kills++;
    }

    public void OnBallGrabbed(Player player)
    {
        ballHolder = player;
        holderPreviousPosition = player.character.position;
        holderPreviousForward = player.character.forward;
    }
    

    public void UpdateBallDistance()
    {
        if (ballHolder != null)
        {
            distanceTraveled += (ballHolder.character.position - holderPreviousPosition).magnitude * Vector3.Dot( holderPreviousForward,ballHolder.character.forward);
            ballHolder.ui.OnDistanceTraveled(distanceTraveled,ballHolder.data.distancePerPoint);
            if (distanceTraveled >= ballHolder.data.distancePerPoint)
            {
                distanceTraveled %= ballHolder.data.distancePerPoint;
                cumulatedPoints++;
                ballHolder.ui.OnPointsChange(cumulatedPoints,kills);
                crowds.SetGlobalValue(cumulatedPoints);
            }
            holderPreviousPosition = ballHolder.character.position;
            holderPreviousForward = ballHolder.character.forward;
        }
    }

    public void ChangeLastCheckPoint(Checkpoint checkpoint)
    {
        lastCheckpoint = checkpoint;
    }

    private void ResetBall()
    {
        ballHolder = null;
        distanceTraveled = 0;
        cumulatedPoints = 0;
        kills = 0;
        holderPreviousPosition = Vector3.zero;
        holderPreviousForward = Vector3.zero;
    }
}
