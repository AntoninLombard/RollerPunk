using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class RankingPanel : MonoBehaviour
{
    [Serializable]
    private struct ScorePanel
    {
        public Image icon;
        public Image background;
        public CanvasGroup canvasGroup;
        public TextMeshProUGUI rank;
        public TextMeshProUGUI currentScore;
    }
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image background;
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

    public void InitRankingDisplay()
    {
        IEnumerator<ScorePanel> scorePanelsEnumerator = scorePanels.GetEnumerator();
        foreach (var playerScore in GameManager.Instance.players)
        {
            scorePanelsEnumerator.MoveNext();
            ScorePanel currentPanel = scorePanelsEnumerator.Current;
            SetPanel(currentPanel,playerScore.Key.color,0);
            currentPanel.canvasGroup.alpha = 1;
        }

        while (scorePanelsEnumerator.MoveNext())
        {
            ScorePanel currentPanel = scorePanelsEnumerator.Current;
            currentPanel.canvasGroup.alpha = 0;
        }
        scorePanelsEnumerator.Dispose();
    }

    void SetPanel(ScorePanel panel,Color color,int score)
    {
        panel.icon.color = color;
        panel.background.color = color;
        panel.rank.color = color;
        panel.currentScore.color = color;
        panel.currentScore.text = score.ToString();
    }

    void UpdateRankingDisplay()
    {
        IEnumerator<ScorePanel> scorePanelsEnumerator = scorePanels.GetEnumerator();
        foreach (var playerScore in GameManager.Instance.players.OrderBy(x => x.Value))
        {
            scorePanelsEnumerator.MoveNext();
            ScorePanel currentPanel = scorePanelsEnumerator.Current;
            SetPanel(currentPanel,playerScore.Key.color,playerScore.Value);
        }
        scorePanelsEnumerator.Dispose();
    }


    private void OnScoreChange(int score)
    {
        UpdateRankingDisplay();
    }

    public void SetColor(Color color)
    {
        text.color = color;
        background.color = color;
    }
}
