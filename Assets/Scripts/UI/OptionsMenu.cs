using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] Toggle low;
    [SerializeField] Toggle medium;
    [SerializeField] Toggle high;

    int stop = -1;


    void Awake()
    {
        if (QualitySettings.GetQualityLevel() == 0) { toggleLow(); }
        else if (QualitySettings.GetQualityLevel() == 1) { toggleMedium(); }
        else if (QualitySettings.GetQualityLevel() == 2) { toggleHigh(); }
    }

    public void changeQuality(int val)
    {
        // prevent toggles from infinitely calling this function as I force their values to change
        if (stop != 0) {
            stop++;
            //Debug.Log("whoa there cowboy " + stop);
            if (stop == 2) { stop = 0; }
            return;
        }

        switch (val)
        {
            case 0:
                toggleLow();
                break;
            case 1:
                toggleMedium();
                break;
            case 2:
                toggleHigh();
                break;
        }
    }

    void toggleLow()
    {
        stop++;
        low.isOn = true;
        low.interactable = false;
        medium.isOn = false;
        medium.interactable = true;
        high.isOn = false;
        high.interactable = true;
        QualitySettings.SetQualityLevel(0, true);
    }

    void toggleMedium()
    {
        stop++;
        low.isOn = false;
        low.interactable = true;
        medium.isOn = true;
        medium.interactable = false;
        high.isOn = false;
        high.interactable = true;
        QualitySettings.SetQualityLevel(1, true);
    }

    void toggleHigh()
    {
        stop++;
        low.isOn = false;
        low.interactable = true;
        medium.isOn = false;
        medium.interactable = true;
        high.isOn = true;
        high.interactable = false;
        QualitySettings.SetQualityLevel(2, true);
    }


}
