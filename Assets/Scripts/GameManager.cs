using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AK.Wwise;
using JetBrains.Annotations;
using TMPro;
using Random = UnityEngine.Random;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] public GameData gameData;
    [SerializeField] private ScoringData scoring;


    [field: Header("Players")]
    [field: SerializeField] public  Dictionary<Player,int> players {get ;private set; }
    [SerializeField] private int nbPlayer;

    [Header("Gameplay")]
    [SerializeField] private bool isPaused;
    [SerializeField] private bool isRaceOn;
    [SerializeField] private float timer;
    [SerializeField] private RespawnPoint raceStart;
    [SerializeField] public List<Checkpoint> checkpoints;
    [SerializeField] private List<RespawnPoint> respawnpoints;

    [Header("Visuals")]
    [SerializeField] private GameObject startLights;

    private Material startLightsMaterial;
    
    [field: Header("Ball & Ball holder")]
    [field: SerializeField] public BallScript ball { get; private set; }

    [SerializeField] public Transform ballSpawn;
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

    [Header("UI elements")]
    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject endGame;
    [SerializeField] private Camera winnerCamera;
    [SerializeField] private TextMeshProUGUI winnerText;
    public delegate void OnScoreChange(int score);
    public delegate void OnPointsChange(int points,int multiplier);
    public delegate void OnBallDropped();
    public delegate void OnBallGrabbed(Player player);
    public delegate void OnDistanceUpdate(float distanceTraveled, float distancePerPoint);
    public delegate void OnKill(Player player);
    public event OnScoreChange onScoreChange;
    public event OnPointsChange onPointsChange;
    public event OnBallDropped onBallDropped;
    public event OnBallGrabbed onBallGrabbed;
    public event OnDistanceUpdate onDistannceUpdate;
    public event OnKill onKill;
    
    private static readonly int EmissiveColor01 = Shader.PropertyToID("_Emissive_color_01");
    private static readonly int EmissiveColor02 = Shader.PropertyToID("_Emissive_color_02");
    private static readonly int EmissiveColor03 = Shader.PropertyToID("_Emissive_color_03");

    private void Awake()
    {
        players = new Dictionary<Player, int>();
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
        EndGame();
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


    public void Scoring()
    {
        players[ballHolder] += cumulatedPoints * currentMultiplier;
        gameData.crowdScoring.Post(gameObject);
        gameData.musicState[4].SetValue();
        onScoreChange?.Invoke(players[ballHolder]);
        
        if (players[ballHolder] >= scoring.maxScore)
        {
            StopGame();
        }
    }
    
    public void BallKill(Player killedPlayer)
    {
        kills++;
        gameData.crowdKill.Post(gameObject);
        if (kills > scoring.killMultipliatorThreshold)
        {
            currentMultiplier = scoring.killMultiplicatorValue;
            onPointsChange?.Invoke(cumulatedPoints,currentMultiplier);
        }
        onKill?.Invoke(killedPlayer);
    }

    public void BallGrabbed(Player player)
    {
        ballHolder = player;
        holderPreviousPosition = player.character.position;
        holderPreviousForward = player.character.forward;
        onBallGrabbed?.Invoke(player);
    }
    
    public void BallDropped()
    {
        ballHolder = null;
        ball.Toggle(true);
        ball.Detach();
        ball.SetEmissiveColor(gameData.ballEmissiveColor);
        RespawnBall();
        onBallDropped?.Invoke();
    }

    // public void OnPlayerReady()
    // {
    //     int count = 0;
    //     foreach (Player player in players.Keys)
    //     {
    //         if (player.isReady)
    //         {
    //             count++;
    //         }
    //     }
    //
    //     if (count == nbPlayer && !isRaceOn)
    //     {
    //         isRaceOn = true;
    //         StartCoroutine(StartCountdown());
    //     }
    //
    // }

    public void UpdateBallDistance()
    {
        if (ballHolder != null)
        {
            distanceTraveled += (ballHolder.character.position - holderPreviousPosition).magnitude * Vector3.Dot( holderPreviousForward,ballHolder.character.forward);
            if (cumulatedPoints == scoring.maxPoint)
            {
                onDistannceUpdate?.Invoke(1,1);
                return;
            }
            onDistannceUpdate?.Invoke(distanceTraveled,scoring.distancePerPoint);
            if (distanceTraveled >= scoring.distancePerPoint)
            {
                distanceTraveled %= scoring.distancePerPoint;
                cumulatedPoints++;
                onPointsChange?.Invoke(cumulatedPoints,currentMultiplier);
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

    public void ResetBall()
    {
        ballHolder = null;
        distanceTraveled = 0;
        cumulatedPoints = 0;
        currentMultiplier = 1;
        kills = 0;
        holderPreviousPosition = Vector3.zero;
        holderPreviousForward = Vector3.zero;
        gameData.score.SetGlobalValue(cumulatedPoints);
        onPointsChange?.Invoke(cumulatedPoints,currentMultiplier);
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
        Transform pos = lastRespawnPoint.ballSpawnPoint;
        int i = 1;
        while (i < respawnpoints.Count && !respawnpoints[(index + i) % respawnpoints.Count].isBallRespawn) { i++; } ;
        pos = respawnpoints[(index + i) % respawnpoints.Count].ballSpawnPoint;
        
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
        //AkSoundEngine.PostEvent("")
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
            else if (nbPlayer == 2)
            {
                switch (player.number)
                {
                    case 0:
                        offsetX = 0;
                        offsetY = 0;
                        break;
                    case 1:
                        offsetX = 0.5f;
                        offsetY = 0;
                        break;
                    default:
                        offsetX = 0;
                        offsetY = 0;
                        break;
                }
                sizeX = 0.5f;
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
                sizeY = 0.5f;
            }
            player.camera.rect = new Rect(offsetX,offsetY,sizeX,sizeY);
            player.ui.ToggleRankingSide(player.number % 2 == 0 ? 1 : -1);
        }
    }


    public void TogglePause(bool isPaused)
    {
        Time.timeScale = isPaused ? 0 : 1;
        this.isPaused = isPaused;
        miniMap.SetActive(!isPaused);
        menu.SetActive(isPaused);
        AkSoundEngine.SetRTPCValue("Pause", isPaused? 1 :0);
        foreach (var player in players.Keys)
        {
            if(this.isPaused)
                player.input.playerInput.SwitchCurrentActionMap("Pause");
            else
            {
                player.input.playerInput.SwitchCurrentActionMap("Driving");
            }
        }
    }
    
    public void ToggleEndGame(bool isEndGame)
    {
        miniMap.SetActive(!isEndGame);
        endGame.SetActive(isEndGame);
        AkSoundEngine.SetRTPCValue("Pause", isEndGame? 1 :0);
        foreach (var player in players.Keys)
        {
            if(isEndGame)
                player.input.playerInput.SwitchCurrentActionMap("Pause");
            else
            {
                player.input.playerInput.SwitchCurrentActionMap("Driving");
            }
        }
        
        foreach (var player in players.Keys)
        {
            player.camera.enabled = !isEndGame;
        }

        winnerCamera.enabled = isEndGame;
        winnerCamera.transform.parent = null;
    }

    public void ReturnToMenu()
    {
        PlayerInputHandler playerInput = PlayerInputHandler.Instance;
        Debug.Log(playerInput);
        Time.timeScale = 1;
        PlayerInputHandler.Instance.ClearPlayers();
        GameObject toDestroy = PlayerInputHandler.Instance.gameObject;
        Destroy(PlayerInputHandler.Instance);
        Destroy(toDestroy);
        AkSoundEngine.SetRTPCValue("Pause", 0);
        AkSoundEngine.SetState("GamePlay_Music", "Game_End");
        SceneManager.LoadScene(0,LoadSceneMode.Single);
    }


    public void Restart()
    {
        lastRespawnPoint = raceStart;
        ballHolder?.combat.dropBall();
        ball.transform.position = ballSpawn.position;
        ball.transform.rotation = ballSpawn.rotation;
        List<Player> playersList = new List<Player>(players.Keys);
        foreach (var player in playersList)
        {
            players[player] = 0;
            onScoreChange?.Invoke(0);
            player.combat.Kill();
            player.ToggleActive(false);
        }
        
        TogglePause(false);
        StartWaitForCountdown();
        AkSoundEngine.SetState("GamePlay_Music", "Game_End");
    }


    public void EndGame()
    {
        ToggleEndGame(true);
        AkSoundEngine.SetState("GamePlay_Music", "Game_End");
        foreach (var player in players.Keys)
        {
            player.combat.Kill();
            player.ToggleActive(false);
        }
        Player winner = players.OrderByDescending(x => x.Value).First().Key;
        winner.combat.OnGrabbingBall.Invoke();
        winner.anime.StartVictoryPose();
        winnerCamera.transform.parent = winner.character;
        winnerCamera.transform.localPosition = 3 * winner.transform.forward + winner.transform.up * 2.5f;
        winnerCamera.transform.LookAt(winner.character.transform.position + winner.character.transform.up,winner.character.transform.up);
        winnerText.text = "Player " + (winner.number+1).ToString() + " wins !";
    }

}
