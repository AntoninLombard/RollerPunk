using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class UI_Animator : MonoBehaviour
{
    // This script handles all UI transitions and animations. Way simpler than creating animators and states for each and every fucking GameObject...
    private float startTime;
    [SerializeField] private float animationDuration = 0.5f;
    private float[] panelPositions = { 300f, 150f, -250f, -300f, -150f, 250f };
    private float panelXCoordinate;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        rectTransform = gameObject.GetComponent<RectTransform>();
        panelXCoordinate = rectTransform.anchoredPosition.x;
    }
    

    public void UIFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    public void UIFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    public void UIPanelMove(int positionIndex)
    {
        // position index values: 0 HIDDEN TOP, 1 PEEKING TOP, 2 DISPLAYED TOP, 3 HIDDEN BOTTOM, 4 PEEKING BOTTOM, 5 DISPLAYED BOTTOM
        StartCoroutine(PanelMove(positionIndex));
    }
    


    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        while(elapsed < animationDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, (elapsed) / animationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        canvasGroup.alpha = 0;
        yield return null;
    }
    
    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while(elapsed < animationDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, (elapsed) / animationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1;
        yield return null;
    }
    
    IEnumerator PanelMove(int position)
    {
        
        float panelDestination = panelPositions[position];
        float panelStartingPosition = rectTransform.anchoredPosition.y;
        float elapsed = 0f;
        while(elapsed < animationDuration)
        {
            rectTransform.anchoredPosition = new Vector2(panelXCoordinate, Mathf.Lerp(panelStartingPosition, panelDestination, elapsed / animationDuration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = new Vector2(panelXCoordinate, panelDestination);
        yield return null;
    }
}

