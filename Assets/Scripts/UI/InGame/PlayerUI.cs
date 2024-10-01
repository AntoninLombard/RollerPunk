using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] public int side;
    [SerializeField] private TextMeshProUGUI textCountdown;
    [SerializeField] private Player player;
    [SerializeField] private NotificationPanel notification;
    [SerializeField] private RankingPanel rankingLeft;
    [SerializeField] private RankingPanel rankingRight;

    public void ToggleRankingSide(int side)
    {
        if (side > 0)
        {
            rankingLeft.gameObject.SetActive(true);
            rankingRight.gameObject.SetActive(false);
        }
        else
        {
            rankingLeft.gameObject.SetActive(false);
            rankingRight.gameObject.SetActive(true);
        }
    }
    
    public void SetCountdown(int value)
    {
        if (value > 0)
        {
            textCountdown.text = value.ToString();
        }
        else
        {
            textCountdown.text = "GO!";
        }
    }

    public void ToggleCountdown(bool isVisible)
    {
        textCountdown.enabled = isVisible;
    }

    public void SetColor(Color color)
    {
        rankingLeft.SetColor(color);
        rankingRight.SetColor(color);
        textCountdown.color = color;
    }
    
    
}
