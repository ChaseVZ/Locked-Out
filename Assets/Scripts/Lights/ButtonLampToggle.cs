using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLampToggle : MonoBehaviour
{
    [SerializeField] Light spotlight;
    [SerializeField] bool startON;

    Material lamp;
    bool ON = false;

    float _spotAngle;

    void Start()
    {
        _spotAngle = spotlight.spotAngle;
        lamp = GetComponent<Renderer>().material;

        if (startON) { ToggleON(); }
        else { ToggleOFF(); }
    }

    public void Toggle()
    {
        if (ON)
        {
            ToggleOFF();
        }
        else
        {
            ToggleON();
        }
    }

    void ToggleON()
    {
        spotlight.spotAngle = _spotAngle;
        spotlight.gameObject.SetActive(true);

        lamp.SetColor("_EmissionColor", Color.white);
        ON = true;
    }

    void ToggleOFF()
    {
        spotlight.spotAngle = 0; // to unreveal anything
        spotlight.gameObject.SetActive(false);

        lamp.SetColor("_EmissionColor", Color.black);
        ON = false;
    }
}
