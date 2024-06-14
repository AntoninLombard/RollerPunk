using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AK.Wwise;
using JetBrains.Annotations;
using Random = UnityEngine.Random;
using Unity.VisualScripting;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] public GameData gameData;
    [SerializeField] private ScoringData scoring;


    [Header("Players")]
    [field: SerializeField] private Dictionary<Player,int> players = new Dictionary<Player, int>();
    [SerializeField] private int nbPlayer;

    [Header("Gameplay")] 
    [SerializeField] private bool isRaceOn;
    [SerializeField] private float timer;
    [SerializeField] private RespawnPoint raceStart;
    [SerializeField] private List<Checkpoint> checkpoints;
    [SerializeField] private List<RespawnPoint> respawnpoints;

    [Header("Visuals")]
    [SerializeField] private GameObject startLights;

    private Material startLightsMaterial;
    
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
    [SerializeField] private RTPC playerNumberRTPC;
    
    
    private static readonly int EmissiveColor01 = Shader.PropertyToID("_Emissive_color_01");
    private static readonly int EmissiveColor02 = Shader.PropertyToID("_Emissive_color_02");
    private static readonly int EmissiveColor03 = Shader.PropertyToID("_Emissive_color_03");

    private void Awake()
    {
        if (Instance == null)
        {
            //DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        
        currentMultiplier = 1;
        lastRespawnPoint = raceStart;
        gameData.crowdStart.Post(gameObject);
        gameData.crowdWaiting.Post(gameObject);
        startLightsMaterial = startLights.GetComponent<MeshRenderer>()?.materials[1];
        startLightsMaterial.SetColor(EmissiveColor01,gameData.countDownColors[0]);
        startLightsMaterial.SetColor(EmissiveColor02,gameData.countDownColors[0]);
        startLightsMaterial.SetColor(EmissiveColor03,gameData.countDownColors[0]);
        StartWaitForCountdown();
    }
    //
    // // Start is called before the first frame update
    // void Start()
    // {
    //     currentMultiplier = 1;
    //     lastRespawnPoint = raceStart;
    //     gameData.crowdStart.Post(gameObject);
    //     gameData.crowdWaiting.Post(gameObject);
    //     startLightsMaterial = startLights.GetComponent<MeshRenderer>()?.materials[1];
    //     
    //     startLightsMaterial.SetColor(EmissiveColor01,gameData.countDownColors[0]);
    //     startLightsMaterial.SetColor(EmissiveColor02,gameData.countDownColors[0]);
    //     startLightsMaterial.SetColor(EmissiveColor03,gameData.countDownColors[0]);
    //
    //     StartWaitForCountdown();
    // }

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

    void StopGame()
    {
        isRaceOn = false;
        foreach (Player player in players.Keys)
        {
            player.ToggleActive(false);
        }
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
        playerNumberRTPC.SetGlobalValue(nbPlayer);
        nbPlayer++;
        RespawnPlayer(player);
        player.listener.SetVolumeOffset(nbPlayer);
    }


    public void OnScoring()
    {
        players[ballHolder] += cumulatedPoints * currentMultiplier;
        ballHolder.ui.OnScoreChange(players[ballHolder]);
        gameData.crowdScoring.Post(gameObject);
        gameData.musicState[4].SetValue();
        if (players[ballHolder] > scoring.maxScore)
        {
            StopGame();
        }
    }
    
    public void OnBallKill()
    {
        kills++;
        gameData.crowdKill.Post(gameObject);
        if (kills > scoring.killMultipliatorThreshold)
        {
            currentMultiplier = scoring.killMultiplicatorValue;
            ballHolder.ui.OnPointsChange(cumulatedPoints,currentMultiplier);
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

        if (count == nbPlayer && !isRaceOn)
        {
            isRaceOn = true;
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
                gameData.scoreUpSound.Post(gameObject);
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
        if(ballHolder != null)
        {
            ballHolder.ui.OnPointsChange(0, 0);
        }
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
            RespawnBall();
        }
    }
    
    public void RespawnBall()
    {
        ResetBall();
        ballHolder = null;
        ball.transform.parent = null;
        int index = respawnpoints.IndexOf(lastRespawnPoint);
        Transform pos = raceStart.transform;
        int i = 1;
        RespawnPoint currentRespawn = raceStart;
        while (i < respawnpoints.Count && !respawnpoints[(index + i) % respawnpoints.Count].isBallRespawn)
        {
            pos = respawnpoints[(index + i) % respawnpoints.Count].ballSpawnPoint;
            i++;
        }
        if(Physics.Raycast(pos.position, -pos.up, out RaycastHit hit, 8f))
        {
            ball.Toggle(true);
            ball.transform.parent = null;
            ball.transform.position = hit.point + hit.normal;
            ball.transform.rotation = pos.rotation;
        }
        
    }

    public void CompareRespawnPoints(RespawnPoint respawnPoint, Player player)
    {
        int indexA = respawnpoints.IndexOf(respawnPoint);
        int indexB = respawnpoints.IndexOf(lastRespawnPoint);
        int N = respawnpoints.Count;
        int distanceAB = (indexA - indexB + N) % N;
        int distanceBA = (indexB - indexA + N) % N;
        player.controller.Rubberbanding(Mathf.Min(distanceAB, distanceBA));
    }


void StartWaitForCountdown()
    {
        StartCoroutine(WaitForCountdown());
    }
    
    
    #region COROUTINES

    IEnumerator StartCountdown()
    {
        gameData.countdownSound.Post(gameObject);

        foreach (Player player in players.Keys)
        {
            player.ui.ToggleCountdown(true);
            player.ui.SetCountdown(3);
        }
        startLightsMaterial.SetColor(EmissiveColor01,gameData.countDownColors[2]);
        startLightsMaterial.SetColor(EmissiveColor02,gameData.countDownColors[3]);
        startLightsMaterial.SetColor(EmissiveColor03,gameData.countDownColors[3]);
        
        yield return new WaitForSeconds(1);

        foreach (Player player in players.Keys)
        {
            player.ui.SetCountdown(2);
        }
        startLightsMaterial.SetColor(EmissiveColor01,gameData.countDownColors[2]);
        startLightsMaterial.SetColor(EmissiveColor02,gameData.countDownColors[2]);
        gameData.countdownSound.Post(gameObject);
        
        yield return new WaitForSeconds(1);
        
        foreach (Player player in players.Keys)
        {
            player.ui.SetCountdown(1);
        }
        
        startLightsMaterial.SetColor(EmissiveColor01,gameData.countDownColors[2]);
        startLightsMaterial.SetColor(EmissiveColor02,gameData.countDownColors[2]);
        startLightsMaterial.SetColor(EmissiveColor03,gameData.countDownColors[2]);
        gameData.countdownSound.Post(gameObject);
        
        yield return new WaitForSeconds(1);
        
        foreach (Player player in players.Keys)
        {
            player.ui.SetCountdown(0);
            player.ToggleActive(true);
        }
        
        startLightsMaterial.SetColor(EmissiveColor01,gameData.countDownColors[1]);
        startLightsMaterial.SetColor(EmissiveColor02,gameData.countDownColors[1]);
        startLightsMaterial.SetColor(EmissiveColor03,gameData.countDownColors[1]);
        gameData.countdownSound.Post(gameObject);

        yield return new WaitForSeconds(0.5f);
        
        foreach (Player player in players.Keys)
        {
            player.ui.ToggleCountdown(false);
        }

        gameData.musicStart.Post(gameObject);
        gameData.crowdRaceStart.Post(gameObject);
    }

    IEnumerator WaitForCountdown()
    {
        yield return new WaitForSeconds(5);
        StartCoroutine(StartCountdown());
    }
    #endregion

    public void SpawnPlayer(PlayerData playerData)
    {
        GameObject playerGameObject = Instantiate(gameData.playerPrefab,null);
        Player player = playerGameObject.GetComponent<Player>();
        player.input.SetInput(playerData.input);
        players.Add(player,0);
        int layer = (int)Math.Log(player.data.playerLayer[nbPlayer], 2);
        player.camera.gameObject.layer = layer;
        foreach(LayerMask mask in player.data.playerLayer.FindAll(x => x != player.data.playerLayer[nbPlayer]))
            player.camera.cullingMask -= player.camera.cullingMask & mask;
        player.setPlayerID(nbPlayer);
        player.number = playerData.nb;
        player.color = playerData.color;
        playerNumberRTPC.SetGlobalValue(nbPlayer);
        nbPlayer++;
        player.Init();
        RespawnPlayer(player);
        player.listener.SetVolumeOffset(nbPlayer);
    }


    public void SplitScreenCamera()
    {
        foreach (Player player in players.Keys)
        {
            float offsetX, offsetY, sizeX, sizeY;
            if (nbPlayer == 1)
            {
                offsetX = 0;
                offsetY = 0;
                sizeX = 1;
                sizeY = 1;
            }
            else
            {
                switch (player.number)
                {
                    case 0:
                        offsetX = 0;
                        offsetY = 0.5f;
                        break;
                    case 1:
                        offsetX = 0.5f;
                        offsetY = 0.5f;
                        break;
                    case 2:
                        offsetX = 0;
                        offsetY = 0;
                        break;
                    case 3:
                        offsetX = 0.5f;
                        offsetY = 0;
                        break;
                    default:
                        offsetX = 0;
                        offsetY = 0;
                        break;
                }
                sizeX = 0.5f;
                sizeY = nbPlayer > 2 ? 0.5f : 1f;
            }
            player.camera.rect = new Rect(offsetX,offsetY,sizeX,sizeY);
        }
    }
}
