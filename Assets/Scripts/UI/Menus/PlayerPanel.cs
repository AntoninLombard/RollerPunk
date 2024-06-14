using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{

    public enum PanelState
    {
        Hide,
        Joining,
        Joined
    }

    public enum PromptState
    {
        Join,
        Ready,
        Controls,
        Waiting
    }
    
    [SerializeField] private int slideDirection;
    [SerializeField] private UI_Animator animator;


    [SerializeField] private Image background;
    [SerializeField] private SkinnedMeshRenderer character;
    [SerializeField] private GameObject joinText;
    [SerializeField] private GameObject waitingText;
    [SerializeField] private GameObject readyText;
    [SerializeField] private GameObject startText;

    
    
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    
    
    
    public void ChangeState(PanelState state)
    {
        switch (state)
        {
            case PanelState.Hide:
                ChangePrompt(PromptState.Join);
                SetColor(Color.black);
                animator.UIPanelMove(slideDirection < 0 ? 0 : 3);
                break;
            case PanelState.Joining:
                ChangePrompt(PromptState.Join);
                animator.UIPanelMove(slideDirection < 0 ? 1 : 4);
                break;
            case PanelState.Joined:
                ChangePrompt(PromptState.Ready);
                animator.UIPanelMove(slideDirection < 0 ? 2 : 5);
                break;
        }
    }
    
    public void ChangePrompt(PromptState state)
    {
        switch (state)
        {
            case PromptState.Join:
                waitingText.SetActive(false);
                joinText.SetActive(true);
                startText.SetActive(false);
                readyText.SetActive(false);
                break;
            case PromptState.Waiting:
                waitingText.SetActive(true);
                joinText.SetActive(false);
                startText.SetActive(false);
                readyText.SetActive(false);
                break;
            case PromptState.Ready:
                waitingText.SetActive(false);
                joinText.SetActive(false);
                startText.SetActive(false);
                readyText.SetActive(true);
                break;
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
    
    
}
