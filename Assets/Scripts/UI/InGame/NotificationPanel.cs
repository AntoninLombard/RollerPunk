using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI points;
    [SerializeField] private TextMeshProUGUI multiplier;
    [SerializeField] private Image scoreBackground;
    [SerializeField] private Image playerIcon;
    [SerializeField] private Image slider;
    [SerializeField] private UI_Animator animator;
    [SerializeField] private List<Image> killIcons;
    private int currentIcon;
    // Start is called before the first frame update
    void OnEnable()
    {
        GameManager.Instance.onBallGrabbed += OnBallGrabbed;
        GameManager.Instance.onBallDropped += OnBallDropped;
        GameManager.Instance.onDistannceUpdate += OnDistannceUpdate;
        GameManager.Instance.onScoreChange += OnScoreChange;
        GameManager.Instance.onPointsChange += OnPointsChange;
        GameManager.Instance.onKill += OnKill;
        animator.UIFadeOut();
    }

    void OnBallGrabbed(Player player)
    {
        SetColor(player.color);
        animator.UIFadeIn();
        animator.UIPanelMove(2);
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
    
    void OnPointsChange(int points)
    {
        this.points.text = points.ToString();
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
        multiplier.color = color;
    }
}
