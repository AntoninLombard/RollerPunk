using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchSettingsHandler : MonoBehaviour
{
    public GameObject[] victoryConditions;
    public int victoryCondition;
    public GameObject[] tracks;
    public int track;
    public Sprite selectedButton;
    public Sprite unselectedButton;

    // Start is called before the first frame update
    void Start()
    {
        track = 0;
        victoryCondition = 0;
        UpdateButtonState();
    }

    public void UpdateVictoryCondition(int vc)
    {
        victoryCondition = vc;
        UpdateButtonState();
    }

    public void UpdateTrack(int t)
    {
        track = t;
        UpdateButtonState();
    }

    void UpdateButtonState()
    {
        foreach (GameObject go in victoryConditions)
        {
            go.GetComponent<Image>().sprite = unselectedButton;
        }
        foreach(GameObject go in tracks)
        {
            go.GetComponent<Image>().sprite = unselectedButton;
        }
        victoryConditions[victoryCondition].GetComponent<Image>().sprite = selectedButton;
        tracks[track].GetComponent<Image>().sprite = selectedButton;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
