using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    /*
     0 - Correct Orientation
     >0 - x * 90 degrees rotated right
     */
    private Dictionary<string, int> Room1_Lamps = new Dictionary<string, int>
    {
        { "Lamp1", 3 },
        { "Lamp2", 1 },
        { "Lamp3", 0 },
        { "Lamp4", 2 },
        { "Lamp5", 0 }
    };


    /* 
     0 - White
     1 - Red
     2 - Blue
     */
    private Dictionary<string, int> Room2_Lamps = new Dictionary<string, int>
    {
        { "Shelf_Lamp1", 0 },
        { "Shelf_Lamp2", 1 },
        { "TableLamp1", 2 },
        { "FloorLamp1", 0 },
        { "FloorLamp2", 0 },
        { "CeilingLamp1", 0 },
        { "CeilingLamp2", 1 },
        { "CeilingLamp3", 1 },
        { "CeilingLamp4", 1 }
    };

    private bool Room1_Complete = false;
    private bool Room2_Complete = false;
    private bool Room3_Complete = false;
    private bool Room4_Complete = false;
    //private bool Lvl1_Done = false;
    private bool winToggle = true;

    public GameObject Room1_LED;
    public GameObject Room2_LED;
    public GameObject Room3_LED;
    public GameObject Room4_LED;

    public GameObject WinDoor;
    public ParticleSystem Confetti;

    public AudioSource taskComplete;
    public AudioSource interact1;
    public AudioSource interact2;
    public AudioSource lvlComplete;

    private void Update()
    {
        if (!Room1_Complete)
            CheckRoom1Status();
        if (!Room2_Complete)
            CheckRoom2Status();
        if (!Room3_Complete)
            CheckRoom3Status();
        if (!Room4_Complete)
            CheckRoom4Status();

        if (Room1_Complete && Room2_Complete && Room3_Complete && Room4_Complete)
            WinCondition();

    }

    public void Room1_PuzzleUpdate(GameObject target)
    {
        /* Check if this is a room1 lamp */
        if (Room1_Lamps.ContainsKey(target.name))
        {
            interact2.Play();
            Room1_Lamps[target.name] = (Room1_Lamps[target.name] - 1) % 4;
            target.transform.Rotate(90, 0, 0);
        }
    }

    public void Room2_PuzzleUpdate(GameObject target)
    {
        if (Room2_Lamps.ContainsKey(target.name))
        {
            Debug.Log(target.name);
            /* If lamp is currently red */
            if (Room2_Lamps[target.name] == 1)
            {
                target.transform.GetChild(0).gameObject.SetActive(false);    /* white shade */
                target.transform.GetChild(1).gameObject.SetActive(false);    /* red shade */
                target.transform.GetChild(2).gameObject.SetActive(true);     /* blue shade */
            }
            /* If lamp is currently blue */
            else if (Room2_Lamps[target.name] == 2)
            {
                target.transform.GetChild(0).gameObject.SetActive(true);     /* white shade */
                target.transform.GetChild(1).gameObject.SetActive(false);    /* red shade */
                target.transform.GetChild(2).gameObject.SetActive(false);    /* blue shade */
            }
            /* If lamp is currently white */
            else
            {
                target.transform.GetChild(0).gameObject.SetActive(false);   /* white shade */
                target.transform.GetChild(1).gameObject.SetActive(true);    /* red shade */
                target.transform.GetChild(2).gameObject.SetActive(false);   /* blue shade */

            }

            interact1.Play();
            Room2_Lamps[target.name] = (Room2_Lamps[target.name] + 1) % 3;
        }
        else
            Debug.Log("not found" + target.name);
    }

    void CheckRoom1Status()
    {
        bool flag = true;
        foreach (var value in Room1_Lamps)
        {
            if (value.Value != 0)
                flag = false;
        }
        Room1_Complete = flag;

        if (Room1_Complete)
        {
            taskComplete.Play();
            Room1_LED.GetComponent<LEDNode>().stopBlinking = true;
            Room1_LED.GetComponent<LEDNode>().emissionColor = new Color(0f, 1f, 0f, 1f);
            Debug.Log("Room1 Complete!");
        }
    }

    void CheckRoom2Status()
    {
        bool flag = true;
        foreach (var value in Room2_Lamps)
        {
            if (value.Value != 1)
                flag = false;
        }
        Room2_Complete = flag;

        if (Room2_Complete)
        {
            taskComplete.Play();
            Room2_LED.GetComponent<LEDNode>().stopBlinking = true;
            Room2_LED.GetComponent<LEDNode>().emissionColor = new Color(0f, 1f, 0f, 1f);
            Debug.Log("Room2 Complete!");
        }
    }

    void CheckRoom3Status()
    {
        Room3_LED.GetComponent<LEDNode>().stopBlinking = true;
        Room3_LED.GetComponent<LEDNode>().emissionColor = new Color(0f, 1f, 0f, 1f);
        Room3_Complete = true;
    }

    void CheckRoom4Status()
    {
        Room4_LED.GetComponent<LEDNode>().stopBlinking = true;
        Room4_LED.GetComponent<LEDNode>().emissionColor = new Color(0f, 1f, 0f, 1f);
        Room4_Complete = true;
    }

    void WinCondition()
    {
        //Debug.Log("win condition met");
        GameObject leftDoor = WinDoor.transform.GetChild(0).gameObject;
        GameObject rightDoor = WinDoor.transform.GetChild(1).gameObject;

        GameObject leftDoorEnd = WinDoor.transform.GetChild(3).gameObject;
        GameObject rightDoorEnd = WinDoor.transform.GetChild(4).gameObject;


        leftDoor.transform.position = Vector3.Lerp(leftDoor.transform.position, leftDoorEnd.transform.position, Time.deltaTime);
        rightDoor.transform.position = Vector3.Lerp(rightDoor.transform.position, rightDoorEnd.transform.position, Time.deltaTime);

        if (winToggle) { 
            lvlComplete.Play();
            Confetti.Play();
        }
        winToggle = false;
        //Lvl1_Done = true;

        //if (0.1f > Mathf.Abs(leftDoorEnd.gameObject.transform.position.z - leftDoor.transform.position.z))
        //   Lvl1_Done = true;
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(5);
    }
}
