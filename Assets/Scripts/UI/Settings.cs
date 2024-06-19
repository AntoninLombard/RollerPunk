using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using TMPro;

public class Settings : MonoBehaviour
{

    public int masterVolume = 80;
    public int musicVolume = 80;
    public int sfxVolume = 80;
    private int volumeSteps = 5;
    public GameObject[] arrowContainers;
    private int buttonToReselect;
    private bool isModifyingValues = false;
    private int currentLanguageIndex = 0;
    private int numberLocales;

    public void Start() 
    {
        numberLocales = LocalizationSettings.AvailableLocales.Locales.Count;
    }

    public void OnArrowContainerSelection(int containerID)
    {
        buttonToReselect = containerID;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        arrowContainers[containerID].GetComponent<RectTransform>().GetChild(1).GetComponent<Image>().enabled = true;
        arrowContainers[containerID].GetComponent<RectTransform>().GetChild(3).GetComponent<Image>().enabled = true;
        isModifyingValues = true;
    }

    private void OnArrowContainerDeselect() 
    {
        arrowContainers[buttonToReselect].GetComponent<Button>().Select();
        arrowContainers[buttonToReselect].GetComponent<RectTransform>().GetChild(1).GetComponent<Image>().enabled = false;
        arrowContainers[buttonToReselect].GetComponent<RectTransform>().GetChild(3).GetComponent<Image>().enabled = false;
        isModifyingValues = false;
    }

    public void OnMovePressed(InputAction.CallbackContext context)
    {
        if (isModifyingValues)
        {
            Vector2 movement = context.ReadValue<Vector2>();
            if (buttonToReselect == 0)
            {
                if (movement.x == -1)
                {
                    OnChangeLanguage(false);
                }
                else if (movement.x == 1)
                {
                    OnChangeLanguage(true);
                }
            }
            else
            {
                if (movement.x == -1)
                {
                    OnChangeVolume(false);
                }
                else if (movement.x == 1)
                {
                    OnChangeVolume(true);
                }
            }
        }
    }

    public void OnChangeLanguage(bool isPositive)
    {
        // Updates the current locale, with rollback if trying to show the next locale with the last one.
        if (isPositive)
        {
            currentLanguageIndex = (currentLanguageIndex + 1) >= numberLocales ? 0 : (currentLanguageIndex + 1);
        } else {
            currentLanguageIndex = (currentLanguageIndex - 1) <= -1 ? (numberLocales - 1) : (currentLanguageIndex - 1);
        }
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[currentLanguageIndex];
    }

    public void OnChangeVolume(bool isPositive)
    {
        // Updates the current volume, clamping the values between 0 and 100.
        switch (buttonToReselect)
        {
            case 1:
                if (isPositive)
                {
                    masterVolume = Mathf.Min(masterVolume + volumeSteps, 100);
                } else {
                    masterVolume = Mathf.Max(masterVolume - volumeSteps, 0);
                }
                break;
            case 2:
                if (isPositive)
                {
                    musicVolume = Mathf.Min(musicVolume + volumeSteps, 100);
                } else {
                    musicVolume = Mathf.Max(musicVolume - volumeSteps, 0);
                }
                break;
            case 3:
                if (isPositive)
                {
                    sfxVolume = Mathf.Min(sfxVolume + volumeSteps, 100);
                } else {
                    sfxVolume = Mathf.Max(sfxVolume - volumeSteps, 0);
                }
                break;
        }
        UpdateValues();
    }

    public void OnConfirmPressed(InputAction.CallbackContext context)
    {
        if (isModifyingValues)
        {
            OnArrowContainerDeselect();
        }
    }

    public void OnReturnPressed(InputAction.CallbackContext context)
    {
        if (isModifyingValues)
        {
            OnArrowContainerDeselect();
        }
    }

    public void UpdateValues()
    {
        arrowContainers[1].GetComponent<RectTransform>().GetChild(2).GetComponent<TextMeshPro>().text = masterVolume.ToString();
        arrowContainers[2].GetComponent<RectTransform>().GetChild(2).GetComponent<TextMeshPro>().text = musicVolume.ToString();
        arrowContainers[3].GetComponent<RectTransform>().GetChild(2).GetComponent<TextMeshPro>().text = sfxVolume.ToString();
    }   
}
