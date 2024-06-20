using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{

    public enum PanelState
    {
        Hide,
        Joining,
        Joined,
        Tutorial1,
        Tutorial2,
        Tutorial3,
        WaitReady,
        Ready,
        Rebind
    }

    public enum PromptState
    {
        Join,
        Next,
        Ready,
        Waiting,
        Rebind
    }

    [SerializeField] private PanelState currentState;
    [SerializeField] private PanelState previousState;
    
    [SerializeField] private int slideDirection;
    [SerializeField] private UI_Animator animator;


    [SerializeField] private Image background;
    [SerializeField] private SkinnedMeshRenderer character;
    [SerializeField] private GameObject joinText;
    [SerializeField] private GameObject nextText;
    [SerializeField] private GameObject waitingText;
    [SerializeField] private GameObject readyText;
    [SerializeField] private GameObject rebindText;
    [SerializeField] private GameObject tutorial1;
    [SerializeField] private GameObject tutorial2;
    [SerializeField] private GameObject tutorial3;

    [SerializeField] private GameObject rebindMenu;
    

    
    
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    public void NextState()
    {
        switch (currentState)
        {
            case PanelState.Hide:
                break;
            case PanelState.Joining:
                ChangeState(PanelState.Joined);
                break;
            case PanelState.Joined:
                ChangeState(PanelState.Tutorial1);
                break;
            case PanelState.Tutorial1:
                ChangeState(PanelState.Tutorial2);
                break;
            case PanelState.Tutorial2:
                ChangeState(PanelState.Tutorial3);
                break;
            case PanelState.Tutorial3:
                ChangeState(PanelState.WaitReady);
                break;
            case PanelState.WaitReady:
                MenuManager.Instance.OnPlayerReady(MenuManager.Instance.panelBindings.First(x => x.Value ==this).Key);
                ChangeState(PanelState.Ready);
                break;
            case PanelState.Ready:
                break;
            case PanelState.Rebind:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void PreviousState()
    {
        switch (currentState)
        {
            case PanelState.Hide:
                break;
            case PanelState.Joining:
                break;
            case PanelState.Joined:
                MenuManager.Instance.CancelPlayerJoin();
                break;
            case PanelState.Tutorial1:
                ChangeState(PanelState.Joined);
                break;
            case PanelState.Tutorial2:
                ChangeState(PanelState.Tutorial1);
                break;
            case PanelState.Tutorial3:
                ChangeState(PanelState.Tutorial2);
                break;
            case PanelState.WaitReady:
                ChangeState(PanelState.Tutorial3);
                break;
            case PanelState.Ready:
                ChangeState(PanelState.WaitReady);
                MenuManager.Instance.OnPlayerReady(MenuManager.Instance.panelBindings.First(x => x.Value ==this).Key);
                break;
            case PanelState.Rebind:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void ChangeState(PanelState state)
    {
        SetActiveAllPanel(false);
        previousState = currentState;
        currentState = state;
        switch (state)
        {
            case PanelState.Hide:
                ChangePrompt(PromptState.Join);
                SetColor(Color.black);
                animator.UIPanelMove(0);
                break;
            case PanelState.Joining:
                ChangePrompt(PromptState.Next);
                animator.UIPanelMove(1);
                break;
            case PanelState.Joined:
                ChangePrompt(PromptState.Next);
                animator.UIPanelMove(2);
                break;
            case PanelState.Tutorial1:
                ChangePrompt(PromptState.Next);
                tutorial1.SetActive(true);
                break;
            case PanelState.Tutorial2:
                ChangePrompt(PromptState.Next);
                tutorial2.SetActive(true);
                break;
            case PanelState.Tutorial3:
                ChangePrompt(PromptState.Next);
                tutorial3.SetActive(true);
                break;
            case PanelState.WaitReady:
                ChangePrompt(PromptState.Ready);
                break;
            case PanelState.Ready:
                ChangePrompt(PromptState.Waiting);
                break;
            case PanelState.Rebind:
                rebindMenu.SetActive(true);
                ChangePrompt(PromptState.Rebind);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    
    public void ChangePrompt(PromptState state)
    {
        SetActiveAllText(false);
        switch (state)
        {
            case PromptState.Join:
                joinText.SetActive(true);
                break;
            case PromptState.Waiting:
                waitingText?.SetActive(true);
                break;
            case PromptState.Ready:
                readyText.SetActive(true);
                break;
            case PromptState.Next:
                nextText.SetActive(true);
                break;
            case PromptState.Rebind:
                rebindText.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }


    public void SetColor(Color color)
    {
        foreach (Material material in character.materials)
        {
            material.SetColor(EmissionColor,color);
        } 
        color.a = 0.5f;
        background.color = color;
    }


    public void SetActiveAllText(bool isActive)
    {
        readyText.SetActive(isActive);
        joinText.SetActive(isActive);
        waitingText.SetActive(isActive);
        nextText.SetActive(isActive);
        readyText.SetActive(isActive);
    }
    
    public void SetActiveAllPanel(bool isActive)
    {
        tutorial1.SetActive(isActive);
        tutorial2.SetActive(isActive);
        tutorial3.SetActive(isActive);
        rebindMenu.SetActive(isActive);
    }

    public void ToggleRebindMenu()
    {
        if(currentState == PanelState.Rebind)
        {
            ChangeState(previousState);
        } else 
        {
            ChangeState(PanelState.Rebind);
        }
    }
    
    
}
