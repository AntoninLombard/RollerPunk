using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelect : MonoBehaviour
{
     [SerializeField] private Image[] playerPanel = new Image[4];
     [SerializeField] private TextMeshProUGUI[] playerText = new TextMeshProUGUI[4];

     private int playerCount = 0;
     
    
     void Awake()
     {
         
     }
     
    // Start is called before the first frame update
    void Start()
    {
        foreach (Image img in playerPanel)
        {
            img.color = Color.gray;
        }

        foreach (TextMeshProUGUI txt in playerText)
        {
            txt.text = "Press A to join";
            txt.color = Color.black;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
