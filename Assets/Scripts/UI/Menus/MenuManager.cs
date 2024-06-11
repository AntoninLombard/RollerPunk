using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private PlayerInput globalInput;
    
    [SerializeField] private int maxNbPlayer;
    [SerializeField] private List<PlayerInput> players;



    [SerializeField] private bool inSettings;
    [SerializeField] private GameObject settings;
    
    [SerializeField] private bool inCredits;
    [SerializeField] private GameObject credits;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    void OnPlayerJoin(PlayerInput playerInput)
    {
        
    }
}
