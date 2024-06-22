using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rank;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI points;
    [SerializeField] private Image scoreBackground;
    [SerializeField] private Image playerIcon;
    [SerializeField] private Image slider;
    [SerializeField] private UI_Animator animator;
    [SerializeField] private List<Image> killIcons;
    private int currentIcon;
    // Start is called before the first frame update
    private void Start()
    {
        animator.UIFadeOut();
    }

    void Awake()
    {
        GameManager.Instance.onBallGrabbed += OnBallGrabbed;
        GameManager.Instance.onBallDropped += OnBallDropped;
        GameManager.Instance.onDistannceUpdate += OnDistannceUpdate;
        GameManager.Instance.onScoreChange += OnScoreChange;
        GameManager.Instance.onPointsChange += OnPointsChange;
        GameManager.Instance.onKill += OnKill;
    }

    private void OnDestroy()
    {
        GameManager.Instance.onBallGrabbed -= OnBallGrabbed;
        GameManager.Instance.onBallDropped -= OnBallDropped;
        GameManager.Instance.onDistannceUpdate -= OnDistannceUpdate;
        GameManager.Instance.onScoreChange -= OnScoreChange;
        GameManager.Instance.onPointsChange -= OnPointsChange;
        GameManager.Instance.onKill -= OnKill;
    }

    // void OnDisable()
    // {
    //     GameManager.Instance.onBallGrabbed -= OnBallGrabbed;
    //     GameManager.Instance.onBallDropped -= OnBallDropped;
    //     GameManager.Instance.onDistannceUpdate -= OnDistannceUpdate;
    //     GameManager.Instance.onScoreChange -= OnScoreChange;
    //     GameManager.Instance.onPointsChange -= OnPointsChange;
    //     GameManager.Instance.onKill -= OnKill;
    // }

    void OnBallGrabbed(Player player)
    {
        currentIcon = 0;
        foreach (var icon in killIcons)
        {
            icon.gameObject.SetActive(false);
        }

        score.text = GameManager.Instance.players[player].ToString();
        points.text = "+" + GameManager.Instance.cumulatedPoints;
        SetColor(player.color);
        animator.UIFadeIn();
        animator.UIPanelMove(1);
    }
    
    void OnBallDropped()
    {
        animator.UIFadeIn();
        animator.UIPanelMove(0);
    }
    
    void OnKill(Player player)
    {
        if (currentIcon < killIcons.Count)
        {
            Image killIcon = killIcons[currentIcon];
            killIcon.gameObject.SetActive(true);
            killIcon.color = player.color;
            currentIcon++;
        }
    }
    
    void OnDistannceUpdate(float distance, float distanceMax)
    {
        slider.fillAmount = distance / distanceMax;
    }
    
    void OnScoreChange(int score)
    {
        this.score.text = score.ToString();
    }
    
    void OnPointsChange(int points, int multiplier)
    {
        this.points.text = "+" + points.ToString() + (multiplier>1? " x" +multiplier.ToString() : "");
    }

    private void Reset()
    {
        SetColor(Color.white);
        slider.fillAmount = 0;
        foreach (var icon in killIcons)
        {
            icon.gameObject.SetActive(false);
        }
    }

    private void SetColor(Color color)
    {
        points.color = color;
        score.color = color;
        foreach (var icon in killIcons)
        {
            icon.color = Color.white;
        }
        playerIcon.color = color;
        slider.color = color;
        scoreBackground.color = color;
        rank.color = color;
    }
}
