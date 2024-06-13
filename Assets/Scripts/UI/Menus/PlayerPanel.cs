using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerPanel : MonoBehaviour
{
    public enum PanelState
    {
        Hide,
        Joining,
        Joined
    }
    
    [SerializeField] private int slideDirection;
    [SerializeField] private UI_Animator animator;
    
    [SerializeField] private TextMeshProUGUI joinText;
    [SerializeField] private TextMeshProUGUI waitingText;
    [SerializeField] private TextMeshProUGUI startText;
    // Start is called before the first frame update

    public void ChangeState(PanelState state)
    {
        switch (state)
        {
            case PanelState.Hide:
                animator.UIPanelMove(slideDirection < 0 ? 0 : 3);
                break;
            case PanelState.Joining:
                animator.UIPanelMove(slideDirection < 0 ? 1 : 4);
                break;
            case PanelState.Joined:
                animator.UIPanelMove(slideDirection < 0 ? 2 : 5);
                break;
        }
    }
    
    
}
