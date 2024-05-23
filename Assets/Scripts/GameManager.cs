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

    [SerializeField] public GameData gameData;
    [SerializeField] private ScoringData scoring;
    
    
    [Header("Players")]
    [SerializeField] private Dictionary<Player,int> players = new Dictionary<Player, int>();
    [SerializeField] private int nbPlayer;

    [Header("Gameplay")] 
    [SerializeField] private bool isRaceOn;
    [SerializeField] private float timer;
    [SerializeField] private RespawnPoint raceStart;
    [SerializeField] private List<Checkpoint> checkpoints;
    [SerializeField] private List<RespawnPoint> respawnpoints;
    
    [field: Header("Ball & Ball holder")]
    [field: SerializeField] public BallScript ball { get; private set; }
    [field: SerializeField] [CanBeNull] public Player ballHolder { get; private set; }
    [SerializeField] private Vector3 holderPreviousPosition;
    [SerializeField] private Vector3 holderPreviousForward;
    [SerializeField] private float distanceTraveled = 0;
    [SerializeField] private int cumulatedPoints;
    [SerializeField] private int kills;
    [SerializeField] private int currentMultiplier;
    [field: SerializeField] [CanBeNull] public RespawnPoint lastRespawnPoint { get; private set; }

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
        currentMultiplier = 1;
        lastRespawnPoint = raceStart;
        gameData.crowdStart.Post(gameObject);
        gameData.musicStart.Post(gameObject);
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
        player.setPlayerID(nbPlayer);
        player.number = nbPlayer;
        player.color = gameData.playerColors[nbPlayer];
        nbPlayer++;
        RespawnPlayer(player);
        player.listener.SetVolumeOffset(nbPlayer);
    }
    public void OnPlayerDeath(Player player)
    {
        Vector3 pos = lastRespawnPoint.spawnPoints[player.number].position;
        Quaternion rot = lastRespawnPoint.spawnPoints[player.number].rotation;
        player.controller.TeleportPlayer(pos,rot);

    }

    public void OnScoring()
    {
        players[ballHolder] += cumulatedPoints * (1+currentMultiplier);
        ballHolder.ui.OnScoreChange(players[ballHolder]);
        gameData.crowdScoring.Post(gameObject);
        ResetBall();
    }
    
    public void OnBallKill()
    {
        kills++;
        if (kills > scoring.killMultipliatorThreshold)
        {
            currentMultiplier = scoring.killMultiplicatorValue;
            ballHolder.ui.OnPointsChange(cumulatedPoints,currentMultiplier);
            gameData.crowdKill.Post(gameObject);
        }
    }

    public void OnBallGrabbed(Player player)
    {
        ballHolder = player;
        holderPreviousPosition = player.character.position;
        holderPreviousForward = player.character.forward;
    }

    public void OnPlayerReady()
    {
        int count = 0;
        foreach (Player player in players.Keys)
        {
            if (player.isReady)
            {
                count++;
            }
        }

        if (count == nbPlayer)
        {
            StartCoroutine(StartCountdown());
        }

    }

    public void UpdateBallDistance()
    {
        if (ballHolder != null)
        {
            distanceTraveled += (ballHolder.character.position - holderPreviousPosition).magnitude * Vector3.Dot( holderPreviousForward,ballHolder.character.forward);
            ballHolder.ui.OnDistanceTraveled(distanceTraveled,scoring.distancePerPoint);
            if (distanceTraveled >= scoring.distancePerPoint)
            {
                distanceTraveled %= scoring.distancePerPoint;
                cumulatedPoints++;
                ballHolder.ui.OnPointsChange(cumulatedPoints,currentMultiplier);
                gameData.score.SetGlobalValue(cumulatedPoints);
            }
            holderPreviousPosition = ballHolder.character.position;
            holderPreviousForward = ballHolder.character.forward;
        }
    }

    public void ChangeLastCheckPoint(RespawnPoint respawnPoint)
    {
        lastRespawnPoint = respawnPoint;
    }

    private void ResetBall()
    {
        ballHolder = null;
        distanceTraveled = 0;
        cumulatedPoints = 0;
        currentMultiplier = 1;
        kills = 0;
        holderPreviousPosition = Vector3.zero;
        holderPreviousForward = Vector3.zero;
        gameData.score.SetGlobalValue(cumulatedPoints);
    }

    public void RespawnPlayer(Player player)
    {
        Transform spawnPoint = lastRespawnPoint.spawnPoints[player.number];
        Vector3 pos = spawnPoint.position;
        Quaternion rot = spawnPoint.rotation;
        if (Physics.Raycast(spawnPoint.position, -spawnPoint.up,out RaycastHit hit, 10f))
        {
            pos = hit.point;
        }

        player.controller.TeleportPlayer(pos,rot);
        if (player == ballHolder)
        {
            ResetBall();
            RespawnBall();
        }
    }
    
    public void RespawnBall()
    {
        ballHolder = null;
        Transform pos = lastRespawnPoint.spawnPoints[Random.Range(0, 4)];
        ball.Toggle(true);
        ball.transform.parent = null;
        ball.transform.position = pos.position;
        ball.transform.rotation = pos.rotation;
    }
    
    
    
    
    #region COROUTINES

    IEnumerator StartCountdown()
    {
        
        foreach (Player player in players.Keys)
        {
            player.ui.ToggleCountdown(true);
            player.ui.SetCountdown(3);
        }

        gameData.countdownSound.Post(gameObject);
        
        yield return new WaitForSeconds(1);

        foreach (Player player in players.Keys)
        {
            player.ui.SetCountdown(2);
        }
        //gameData.countdownSound.Post(gameObject);
        
        yield return new WaitForSeconds(1);
        
        foreach (Player player in players.Keys)
        {
            player.ui.SetCountdown(1);
        }
        //gameData.countdownSound.Post(gameObject);
        
        yield return new WaitForSeconds(1);
        
        foreach (Player player in players.Keys)
        {
            player.ui.SetCountdown(0);
            player.ToggleActive(true);
        }
        //gameData.countdownSound.Post(gameObject);

        yield return new WaitForSeconds(0.5f);
        
        foreach (Player player in players.Keys)
        {
            player.ui.ToggleCountdown(false);
        }
    }
    #endregion
}
