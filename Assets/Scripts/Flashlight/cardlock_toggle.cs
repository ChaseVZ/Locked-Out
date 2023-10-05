using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardlock_toggle : MonoBehaviour
{
    [SerializeField] CardType c;
    [SerializeField] GameObject active;
    [SerializeField] GameObject deative;
    [SerializeField] GameObject _light;
    [SerializeField] bool startDeactive;

    private void Awake()
    {
        if (startDeactive)
        {
            toggleOff();
        }
        else
        {
            toggleOn();
        }
    }

    public void checkToggle(CardType targetC)
    {
        if (c == targetC)
        {
            toggleOn();
        }
        else
        {
            toggleOff();
        }
    }

    void toggleOff()
    {
        active.gameObject.SetActive(false);
        deative.gameObject.SetActive(true);
        _light.gameObject.SetActive(false);
    }

    void toggleOn()
    {
        active.gameObject.SetActive(true);
        deative.gameObject.SetActive(false);
        _light.gameObject.SetActive(true);
    }
}
