using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapHandler : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float scale;
    [SerializeField] private GameObject playerIcon; 
    private Dictionary<Player,RectTransform> playersPos;
    [SerializeField] private GameObject ballIcon;
    [SerializeField] private RectTransform ballPos;
    [SerializeField] private GameObject checkpointIcon;
    private List<RectTransform> checkpointsPos;
    
    // Start is called before the first frame update
    void Start()
    {
        playersPos = new Dictionary<Player, RectTransform>();
        checkpointsPos = new List<RectTransform>();
        foreach (var checkpoint in GameManager.Instance.checkpoints)
        {
            Vector3 checkpointPosition = checkpoint.transform.position;
            checkpointPosition = checkpointPosition * scale + offset;
            checkpointPosition = new Vector3(checkpointPosition.x, 0, checkpointPosition.z);
            GameObject currentCheckpoint = Instantiate(checkpointIcon,transform);
            RectTransform currentCheckpointPos = currentCheckpoint.GetComponent<RectTransform>();
            checkpointsPos.Add(currentCheckpoint.GetComponent<RectTransform>());
            currentCheckpointPos.localPosition = checkpointPosition;
            currentCheckpointPos.localRotation = Quaternion.Euler(-90,90,0);
        }

        foreach (var player in GameManager.Instance.players.Keys)
        {
            Vector3 playerPosition = player.character.transform.position;
            playerPosition = playerPosition * scale + offset;
            playerPosition = new Vector3(playerPosition.x, 0, playerPosition.z);
            GameObject currentPlayer = Instantiate(playerIcon,transform);
            playersPos.Add(player,currentPlayer.GetComponent<RectTransform>());
            playersPos[player].localPosition = playerPosition;
            playersPos[player].localRotation = Quaternion.Euler(-90,90,0);
            currentPlayer.GetComponent<Image>().color = player.color;
        }
        Vector3 ballPosition = GameManager.Instance.ball.transform.position;
        ballPosition = ballPosition * scale + offset;
        ballPosition = new Vector3(ballPosition.x, 0, ballPosition.z);
        GameObject ball = Instantiate(ballIcon,transform);
        ballPos = ball.GetComponent<RectTransform>();
        ballPos.localPosition = ballPosition;
        ballPos.localRotation = Quaternion.Euler(-90,90,0);
        ballPos = ball.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        foreach (var player in playersPos)
        {
            Vector3 playerPosition = player.Key.character.transform.position;
            playerPosition = playerPosition * scale + offset;
            playerPosition = new Vector3(playerPosition.x, 0, playerPosition.z);
            player.Value.localPosition = playerPosition;

        }
        Vector3 ballPosition = GameManager.Instance.ball.transform.position;
        ballPosition = ballPosition * scale + offset;
        ballPosition = new Vector3(ballPosition.x, 0, ballPosition.z);
        ballPos.localPosition = ballPosition;
    }
}
