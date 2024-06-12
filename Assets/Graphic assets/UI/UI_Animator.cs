using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class UI_Animator : MonoBehaviour
{
    // This script handles all UI transitions and animations. Way simpler than creating animators and states for each and every fucking GameObject...
    private float startTime;
    private float animationDuration = 1;
    private bool animating = false;
    private bool fadingIn = false;
    private bool fadingOut = false;
    private bool panelMoving = false;
    private int[] panelPositions = { 300, 150, -250, -300, -150, 250 };
    private int panelDestination;
    private float panelStartingPosition;
    private float panelXCoordinate;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        rectTransform = gameObject.GetComponent<RectTransform>();
        panelXCoordinate = rectTransform.anchoredPosition.x;
    }

    void Update()
    {
        if (animating)
        {
            if (fadingIn)
            {
                if (startTime + animationDuration > Time.time)
                {
                    canvasGroup.alpha = Mathf.Lerp(0, 1, (Time.time - startTime) / (startTime + animationDuration));
                } else
                {
                    canvasGroup.alpha = 1;
                    CancelUIAnimations();
                }
            } else if (fadingOut)
            {
                if (startTime + animationDuration > Time.time)
                {
                    canvasGroup.alpha = Mathf.Lerp(1, 0, (Time.time - startTime) / (startTime + animationDuration));
                }
                else
                {
                    canvasGroup.alpha = 0;
                    CancelUIAnimations();
                }
            } else if (panelMoving)
            {
                if (startTime + animationDuration > Time.time)
                {
                    rectTransform.anchoredPosition = new Vector2(panelXCoordinate, Mathf.Lerp(panelStartingPosition, panelDestination, (Time.time - startTime) / (startTime + animationDuration)));
                }
                else
                {
                    rectTransform.anchoredPosition = new Vector2(panelXCoordinate, panelDestination);
//                    Debug.Log("lmao even");
                    CancelUIAnimations();
                }
            }
        }
    }

    public void UIFadeIn()
    {
        startTime = Time.time;
        animating = true;
        fadingIn = true;
        fadingOut = false;
    }

    public void UIFadeOut()
    {
        startTime = Time.time;
        animating = true;
        fadingOut = true;
        fadingIn = false;
    }

    public void UIPanelMove(int positionIndex)
    {
        // Changes position of the player panels in CharaSelect, with an option for instant positionning in case of returning to the CharaSelect.
        // position index values: 0 HIDDEN TOP, 1 PEEKING TOP, 2 DISPLAYED TOP, 3 HIDDEN BOTTOM, 4 PEEKING BOTTOM, 5 DISPLAYED BOTTOM
        CancelUIAnimations();
        panelDestination = panelPositions[positionIndex];
        panelStartingPosition = rectTransform.anchoredPosition.y;
        startTime = Time.time;
        animating = true;
        panelMoving = true;
    }

    private void CancelUIAnimations()
    {
        animating = false;
        fadingIn = false;
        fadingOut = false;
        panelMoving = false;
    }
}
