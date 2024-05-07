using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textScore;
    [SerializeField] private TextMeshProUGUI textPoints;
    [SerializeField] private Slider slider;

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
        slider.value = distance % distanceMax;
    }
}
