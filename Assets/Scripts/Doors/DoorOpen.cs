using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorColor
{
    Blue, Red, Yellow, Green, NONE, Multi
};

public class DoorOpen : MonoBehaviour
{


    enum DoorType
    {
        KeyCard, Button, Intro
    };

    float distanceToPlayer;
    float minDistance = 5f;
    int isNearby;

    Animator animator;
    GameObject player;
    Renderer rend;
    Renderer doorLightRend1;
    Renderer doorLightRend2;
    Light pillarLight1;
    Light pillarLight2;

    [SerializeField] public DoorColor doorC;
    [SerializeField] DoorType doorT;
    [SerializeField] bool useDoorLock;
    [SerializeField] bool locked;
    [SerializeField] bool startOpen;

    List<GameObject> CardLocks = new List<GameObject>();

    AudioSource success1;
    AudioSource doorOpen;
    AudioSource doorClose;

    GameObject[] conditions;

    Color Mahogny = new Color(195/255f, 20/255f, 0, 0.5f);
    Color SteelBlue = new Color(70/255f, 130/255f, 180/255f, 0.5f);
    Color RoyalBlue = new Color(65 / 255f, 105 / 255f, 255 / 255f, 0.5f);
    Color Yellow = new Color(253/255f, 218/255f, 13/255f, 0.5f);
    Color Emerald = new Color(80/255f, 200/255f, 120/255f, 0.5f);

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        isNearby = Animator.StringToHash("character_nearby");
        success1 = GameObject.Find("Success1").GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        rend = GetComponent<Renderer>();

        if (doorC == DoorColor.Red && doorT == DoorType.Button) { rend.material.color = Mahogny; }
        if (doorC == DoorColor.Blue && doorT == DoorType.Button) { rend.material.color = RoyalBlue; }
        if (doorC == DoorColor.Green && doorT == DoorType.Button) { rend.material.color = Emerald; }
        if (doorC == DoorColor.Yellow && doorT == DoorType.Button) { rend.material.color = Yellow; }
        

        // Will only grab if they exist
        int i = 0;
        foreach (Transform child in this.transform)
        {
            if (child.tag == "DoorLED")
            {
                if (i == 0)
                {
                    doorLightRend1 = child.gameObject.GetComponent<Renderer>();
                    pillarLight1 = child.transform.GetChild(0).GetComponent<Light>();
                    i = 1;
                }
                else { 
                    doorLightRend2 = child.gameObject.GetComponent<Renderer>();
                    pillarLight2 = child.transform.GetChild(0).GetComponent<Light>();
                    i = 2;
                }
            }
        }

        // IF they exist
        if (i == 2)
        {
            if (startOpen) { setLightPillarsGreen(); }
            else { setLightPillarsRed(); }
        }
   
        if (doorT == DoorType.KeyCard || doorT == DoorType.Intro || doorT == DoorType.Button)
        {
            AudioSource[] audios = GetComponents<AudioSource>();
            if (audios.Length != 0)
            {
                doorOpen = audios[0];
                doorClose = audios[1];
            }
        }
        if (doorT == DoorType.Button) 
        { 
            if (startOpen) { animator.SetBool(isNearby, true); }
        }

        foreach (Transform child in transform)
        {
            if (child.tag == "CardLock") { CardLocks.Add(child.gameObject); }
        }

        conditions = GameObject.FindGameObjectsWithTag("Condition");
    }

    void Update()
    {
        /* initial conditions to meet for core door functions */
        if (!passInitConditions()) { return; }

        //Debug.Log(name + "pass init");
        
        /* check door types */
        if (doorT == DoorType.Button)
        {
            return;
        }
        else if (doorT == DoorType.Intro)
        {
            checkIntroDoorOpen();
        }
        else if (doorT == DoorType.KeyCard)
        {
            checkKeyCardDoorOpen();
        }
    }

    bool passInitConditions()
    {
        if (player == null) { return false; }
        if (locked) { DoorClose();  return false; }
        if (!passConditions()) { DoorClose(); return false; }
        if (!checkIfClose()) 
        {
            /* we left range so this is now false for all locks */
            foreach (GameObject CardLock in CardLocks) { CardLock.GetComponent<CardLock>().setJustTookCard(false); }
            DoorClose();
            return false; 
        }
        return true;
    }

    /* FOR NOW: checks for all "Rotateable" objects. True if they are rotated correctly */
    bool passConditions()
    { 
        if(conditions == null || conditions.Length == 0) { return true; }
        
        else 
        {
            foreach (GameObject cond in conditions)
            {
                if(Mathf.Round(cond.transform.rotation.eulerAngles.z).ToString() != cond.name) { return false; }
            }
            return true;
        }
    }

    bool checkIfClose()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        return distanceToPlayer < minDistance;
    }

    void DoorClose()
    {
        if (doorT != DoorType.Button && !doorClose.isPlaying && animator.GetBool(isNearby))
        {
            doorClose.Play();
            animator.SetBool(isNearby, false);
        }
    }

    public void placeBlock()
    {
        foreach (Transform child in this.transform)
        {
            if (child.tag == "Block")
                child.gameObject.SetActive(true);
        }
    }

    public void removeBlock()
    {
        foreach (Transform child in this.transform)
        {
            if (child.tag == "Block")
                child.gameObject.SetActive(false);
        }
    }

    void checkIntroDoorOpen()
    {
        if (!animator.GetBool(isNearby) && !doorOpen.isPlaying)
        {
            doorOpen.Play();
            animator.SetBool(isNearby, true);
        }
    }

    void checkKeyCardDoorOpen()
    {
        /* when door is closed or fully open then: */
        /* if we just took the card out, dont try to put it right back in */
        if (!animator.GetBool(isNearby) && !doorOpen.isPlaying)  
        {
            /* for any lock on this door, if card was just taken then dont continue */
            foreach (GameObject CardLock in CardLocks) { if(CardLock.GetComponent<CardLock>().getJustTookCard()) { return; }}

            if (checkAreLocksFilled()) 
            {
                animator.SetBool(isNearby, true);
                doorOpen.Play();
                if (doorLightRend1.materials[1].color == Color.red) { doorSuccess(); }
            }
            //else if (checkCanFillLocks()) /* auto insert into door */
            //{
            //    animator.SetBool(isNearby, true);
            //    doorOpen.Play();
            //    insertLocks();

            //    doorSuccess();         
            //}
            else
            {
                DoorClose();
            }
        }
        else if (animator.GetBool(isNearby) && !checkAreLocksFilled())
        {
            DoorClose();
            swapLightPillarColors();
        }

    }

    void doorSuccess()
    {
        success1.Play();
        swapLightPillarColors();
    }

    /* for this given door, check each of its locks and place cards if we have them */
    /* returns true if all locks can be filled */
    bool insertLocks()
    {
        bool res = true;
        foreach (GameObject CardLock in CardLocks)
        {
            //Debug.Log("inserting into " + CardLock.name);
            if(CardLock.GetComponent<CardLock>().TryInsert() == false) { res = false; }
        }
        return res;
    }

    /* checks each lock on this door 
   returns TRUE: player has cards needed to fill missing locks (if any) */
    bool checkCanFillLocks()
    {
        foreach (GameObject CardLock in CardLocks)
        {
            if (!CardLock.GetComponent<CardLock>().hasCard() && !player.GetComponent<Inventory>().hasCardOfType(CardLock.GetComponent<CardLock>().whatType())) { return false; }
        }
        Debug.Log("can fill");
        return true;
    }

    /* returns TRUE: all door's locks are already filled */
    bool checkAreLocksFilled()
    {
        foreach (GameObject CardLock in CardLocks)
        {
            if (!CardLock.GetComponent<CardLock>().hasCard()) { return false; }
        }
        return true;
    }

    /* TRUE: door opens */
    public bool ButtonPressed()
    {
        /* success */
        if (!animator.GetBool(isNearby))
        {
            doorOpen.Play();
            animator.SetBool(isNearby, true);
            return true;
        }
        /* fail */
        else
        {
            doorClose.Play();
            animator.SetBool(isNearby, false);
            return false;
        }
    }

    public void lockDoor()
    {
        if (useDoorLock) { locked = true; }      
    }

    public void unlockDoor()
    {
        //Debug.Log(name + " has been unlocked");
        locked = false;
    }

    void setLightPillarsGreen()
    {
        doorLightRend1.materials[1].color = Color.green;
        doorLightRend1.materials[1].SetColor("_EmissionColor", Color.green);
        doorLightRend2.materials[1].color = Color.green;
        doorLightRend2.materials[1].SetColor("_EmissionColor", Color.green);
        pillarLight1.color = Color.green;
        pillarLight2.color = Color.green;
    }

    void setLightPillarsRed()
    {
        doorLightRend1.materials[1].color = Color.red;
        doorLightRend1.materials[1].SetColor("_EmissionColor", Color.red);
        doorLightRend2.materials[1].color = Color.red;
        doorLightRend2.materials[1].SetColor("_EmissionColor", Color.red);
        pillarLight1.color = Color.red;
        pillarLight2.color = Color.red;
    }

    void swapLightPillarColors()
    {
        if (doorLightRend1.materials[1].color == Color.red)
        {
            setLightPillarsGreen();
        }
        else
        {
            setLightPillarsRed();
        }
    }


}
