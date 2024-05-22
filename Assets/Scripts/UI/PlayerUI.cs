using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Player player;
    
    [SerializeField] private TextMeshProUGUI textScore;
    [SerializeField] private TextMeshProUGUI textPoints;
    [SerializeField] private TextMeshProUGUI textCountdown;
    [SerializeField] private Slider slider;
    [SerializeField] private Image sliderFill;
    
    
    
    public void OnScoreChange(int newScore)
    {
        textScore.text = "Score: " + newScore;
        textPoints.text = "0";
        slider.value = 0;
    }
    
    public void OnPointsChange(int points,int multiplier)
    {
        textPoints.text = points + " x " + multiplier;
    }

    public void OnDistanceTraveled(float distance, float distanceMax)
    {
        slider.value = distance / distanceMax;
    }

    public void SetColor(Color color)
    {
        textScore.color = color;
        textScore.color = color;
        textCountdown.color = color;
        sliderFill.color = color;
    }


    public void SetReady(bool isReady)
    {
        if (isReady)
        {
            ToggleCountdown(true);
            textCountdown.text = "READY";
        }
        else
        {
            ToggleCountdown(false);
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
}
