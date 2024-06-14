using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RankingPanel : MonoBehaviour
{
    [Serializable]
    private struct ScorePanel
    {
        public CanvasGroup canvasGroup;
        public TextMeshProUGUI rank;
        public TextMeshProUGUI currentScore;
    }
    
    
    [SerializeField] private List<ScorePanel> scorePanels;
    // Start is called before the first frame update
    
    private void OnEnable()
    {
        GameManager.Instance.onScoreChange += OnScoreChange;
    }

    private void Start()
    {
        InitRankingDisplay();
    }


    void InitRankingDisplay()
    {
        IEnumerator<ScorePanel> scorePanelsEnumerator = scorePanels.GetEnumerator();
        foreach (var playerScore in GameManager.Instance.players.OrderBy(x => x.Value))
        {
            ScorePanel currentPanel = scorePanelsEnumerator.Current;
            currentPanel.rank.color = playerScore.Key.color;
            currentPanel.currentScore.color = playerScore.Key.color;
            currentPanel.currentScore.text = "0";
            currentPanel.canvasGroup.alpha = 1;
            scorePanelsEnumerator.MoveNext();
        }

        while (scorePanelsEnumerator.MoveNext())
        {
            ScorePanel currentPanel = scorePanelsEnumerator.Current;
            currentPanel.canvasGroup.alpha = 0;
        }
        scorePanelsEnumerator.Dispose();
    }

    void UpdateRankingDisplay()
    {
        IEnumerator<ScorePanel> scorePanelsEnumerator = scorePanels.GetEnumerator();
        foreach (var playerScore in GameManager.Instance.players.OrderBy(x => x.Value))
        {
            ScorePanel currentPanel = scorePanelsEnumerator.Current;
            currentPanel.rank.color = playerScore.Key.color;
            currentPanel.currentScore.color = playerScore.Key.color;
            currentPanel.currentScore.text = playerScore.Value.ToString();
            scorePanelsEnumerator.MoveNext();
        }
        scorePanelsEnumerator.Dispose();
    }


    private void OnScoreChange(int score)
    {
        //UpdateRankingDisplay();
    }
}
