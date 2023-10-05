using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject flashlight;
    public GameObject lights;
    public Light redLight;
    public Light whiteLight;

    AudioSource toggleClick;

    public static bool flashlightActive = false;
    bool flashlightOn = false;
    bool white = true;
    float whiteAngle;
    float redAngle;

    GameObject player;


    private void Awake()
    {
        whiteAngle = whiteLight.spotAngle;
        redAngle = redLight.spotAngle;
        player = GameObject.FindWithTag("Player");
    }

    private void Start()
    {
        redLight.spotAngle = 0;
        whiteLight.spotAngle = 0;

        toggleClick = flashlight.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (numLights() > 0) 
                activateFlashlight();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && flashlightActive)
        {
            toggleFlashlight();
            toggleClick.Play();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && flashlightOn && flashlightActive)
        {
            if (numLights() > 1)
            {
                toggleClick.Play();
                changeColor();
            }
        }

        //Debug.Log(redLight.spotAngle);
        //Debug.Log(whiteLight.spotAngle);

    }

    int numLights()
    {
        int res = 0;
        if (player.GetComponent<Inventory>().getWhiteFlashlight()) { res++; }
        if (player.GetComponent<Inventory>().getRedFlashlight()) { res++; }
        return res;
    }

    void activateFlashlight()
    {
        flashlight.SetActive(!flashlightActive);
        flashlightActive = !flashlightActive;
        MagicRevealUpdates();

        if (!flashlightActive)
        {
            if (white)
            {
                turnOffWhite();
                turnOffRed();
            }
            else
            {
                turnOffRed();
                turnOffWhite();
            }
        }
    }

    void toggleFlashlight()
    {
        lights.SetActive(!flashlightOn);
        flashlightOn = !flashlightOn;
        MagicRevealUpdates();
    }

    void MagicRevealUpdates()
    {
        if (!flashlightOn || !flashlightActive)
        {
            whiteLight.spotAngle = 0;
            redLight.spotAngle = 0;
        }
        else
        {
            if (white)
            {
                whiteLight.spotAngle = whiteAngle; 
            }
            else
            {
                redLight.spotAngle = redAngle;
            }
        }
    }

    void changeColor()
    {
        if (white)
        {
            turnOffWhite();
        }
        else
        {
            turnOffRed();
        }
    }

    public void turnOffWhite()
    {
        whiteLight.spotAngle = 0;
        redLight.spotAngle = redAngle;
        whiteLight.gameObject.SetActive(false);
        redLight.gameObject.SetActive(true);
        white = false;
    }

    public void turnOffRed()
    {
        whiteLight.spotAngle = whiteAngle;
        redLight.spotAngle = 0;
        whiteLight.gameObject.SetActive(true);
        redLight.gameObject.SetActive(false);
        white = true;
    }

    public void resetReveals()
    {
        whiteLight.spotAngle = whiteAngle;
        redLight.spotAngle = redAngle;
        flashlight.transform.position += new Vector3(0, -100, 0);
        flashlight.SetActive(true);
        whiteLight.gameObject.SetActive(true);
        redLight.gameObject.SetActive(true);
        StartCoroutine(waitTime(0.01f));
    }

    private IEnumerator waitTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        whiteLight.gameObject.SetActive(false);
        redLight.gameObject.SetActive(false);
        flashlight.transform.position += new Vector3(0, 100, 0);
        flashlight.SetActive(false);
        whiteLight.spotAngle = 0;
        redLight.spotAngle = 0;
    }
}
