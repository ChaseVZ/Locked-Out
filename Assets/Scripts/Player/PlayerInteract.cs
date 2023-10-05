using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

// attach to objects that should pickup-able
// an object is pickup-able if it has a collider + rigidbody + moveable (layermask)
public class PlayerInteract : MonoBehaviour
{
    public float pickUpRange = 3.5f;
    public float moveForce = 250f;
    public float maxDistance = 10f;
    public float throwPower = 2500f;

    public Transform holdParent;
    public GameObject teleportLocation;  /* brings player back to start room */

    public AudioSource backgroundMusic;

    GameObject player;
    GameObject heldObj;
    GameObject mainCam;
    GameObject gameManager;

    AudioSource itemGrab;

    [SerializeField] bool GODMODE;


    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        mainCam = GameObject.FindWithTag("MainCamera");

        itemGrab = GameObject.Find("ItemGrab").GetComponent<AudioSource>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager");

        backgroundMusic.Play();
    }
    void Update()
    {
        Debug.DrawRay(mainCam.transform.position, mainCam.transform.TransformDirection(Vector3.forward) * pickUpRange);
        Vector3 forward = mainCam.transform.TransformDirection(Vector3.forward) * pickUpRange;

        if (Input.GetKeyDown(KeyCode.Backslash) && GODMODE) { gameManager.GetComponent<GameManager>().nextPuzzle(); }

        /* Don't allow player to interact with menu open or tooltip active */
        if (PauseMenu.GameIsPaused || TooltipManager.waitingForInput) { return;  }

        /* Left Click - Pickup/Rotate (Don't allow while flashlight is out) */
        if (Input.GetKeyDown(KeyCode.Mouse0) && !Flashlight.flashlightActive)
        {
            if (heldObj == null)
            {
                RaycastHit hit;       
                if (Physics.Raycast(mainCam.transform.position, mainCam.transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    Interact(hit.transform.gameObject, true);
                }
            }
            else
            {
                DropObject();
            }
        }

        /* E - Interact // used to collect keycards, push door buttons */
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(mainCam.transform.position, mainCam.transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
            {
                Interact(hit.transform.gameObject, false);
            }
        }

        /* Right Click - throw picked up obj */
        if (Input.GetKeyDown(KeyCode.Mouse1) && heldObj != null)
        {
            ThrowObject();
        }
        
        /* Passive - if holding object, move it */
        if (heldObj != null)
        {
            MoveObject(null);
        }


        //if (Input.GetKeyDown("1")) // Fast Quality
        //{
        //    QualitySettings.SetQualityLevel(0, true);
        //    Cursor.visible = false;
        //}
        //else if (Input.GetKeyDown("2")) // Medium Quality
        //{
        //    QualitySettings.SetQualityLevel(1, true);
        //    Cursor.visible = false;
        //}
        //else if (Input.GetKeyDown("3")) // High Quality
        //{
        //    QualitySettings.SetQualityLevel(3, true);
        //    Cursor.visible = false;
        //}


    }

    void Interact(GameObject target, bool leftClick)
    {
        if (target.CompareTag("Moveable") && leftClick)
        {
            MoveObject(target);
        }
        else if (target.CompareTag("Rotateable"))
        {
            RotateObject(target);
            target.GetComponent<AudioSource>().Play();
        }
        else if (target.CompareTag("ColorToggle"))
        {
            //Level1_Manager.Room2_PuzzleUpdate(target);
        }
        else if (target.CompareTag("RedKeyCard"))
        {
            itemGrab.Play();
            player.GetComponent<Inventory>().setRed(true);
            Destroy(target);
        }
        else if (target.CompareTag("BlueKeyCard"))
        {
            itemGrab.Play();
            player.GetComponent<Inventory>().setBlue(true);
            Destroy(target);
        }
        else if (target.CompareTag("YellowKeyCard"))
        {
            itemGrab.Play();
            player.GetComponent<Inventory>().setYellow(true);
            TooltipManager.instance.showTooltipObj(Tooltip.Keycard); // first keycard is yellow
            Destroy(target);
        }
        else if (target.CompareTag("GreenKeyCard"))
        {
            itemGrab.Play();
            player.GetComponent<Inventory>().setGreen(true);
            Destroy(target);
        }
        else if (target.CompareTag("BlackKeyCard"))
        {
            itemGrab.Play();
            player.GetComponent<Inventory>().setBlack(true);
            Destroy(target);
        }
        else if (target.CompareTag("Button"))
        {
            target.GetComponent<ButtonPress>().interact();
        }
        else if (target.CompareTag("CardLock"))
        {
            //Debug.Log("taking or placing " + target.name);
            target.GetComponent<CardLock>().TakeorPlaceCard();
        }
        else if (target.CompareTag("FlashlightWhite"))
        {
            itemGrab.Play();
            player.GetComponent<Inventory>().setWhiteFlashlight(true);
            TooltipManager.instance.showTooltipObj(Tooltip.W_Flashlight);
            Destroy(target);
        }
        else if (target.CompareTag("FlashlightRed"))
        {
            itemGrab.Play();
            player.GetComponent<Inventory>().setRedFlashlight(true);
            TooltipManager.instance.showTooltipObj(Tooltip.R_Flashlight);
            Destroy(target);
        }
        else if (target.CompareTag("Handle"))
        {
            target.GetComponent<AudioSource>().PlayOneShot(target.GetComponent<AudioSource>().clip);
            //target.GetComponent<AudioSource>().Play();
        }
        else if (target.CompareTag("LampButton"))
        {
            target.GetComponent<ButtonPressLamp>().interact();
        }
        else if (target.CompareTag("Twistable"))
        {
            if (!target.GetComponent<Animator>().GetBool("Interact")) { target.GetComponent<Animator>().SetBool("Interact", true); target.GetComponent<AudioSource>().Play(); sendTwistEvent(target.name); }  
        }
    }

    void sendTwistEvent(string name)
    {
        int value = findIntFromString(name);

        if (value != -1) { gameManager.GetComponent<GameManager>().twistEvent(value); }
    }

    // -1 means no integer value found (assumes positive numbers only needed)
    int findIntFromString(string s)
    {
        string[] digits = Regex.Split(s, @"\D+");
        foreach (string value in digits)
        {
            int number;
            if (int.TryParse(value, out number))
            {
                Debug.Log(value);
                return number;
            }
        }
        return -1;
    }

    /* target only used for setting held object */
    void MoveObject(GameObject target)
    {
        if (target != null)
        {
            if (target.GetComponent<Rigidbody>())
            {
                Rigidbody objRig = target.GetComponent<Rigidbody>();
                objRig.useGravity = false;
                objRig.drag = 10;
                objRig.transform.parent = holdParent;

                heldObj = target;
            }
        }
        if (Vector3.Distance(heldObj.transform.position, holdParent.position) > 0.1f)
        {
            Vector3 moveDirection = (holdParent.position - heldObj.transform.position);
            heldObj.GetComponent<Rigidbody>().AddForce(moveDirection * moveForce);
        }

        // Drop object if it gets too far (ex: stuck behind obj)
        if (Vector3.Distance(heldObj.transform.position, player.transform.position) > maxDistance)
            DropObject();
    }

    void RotateObject(GameObject target)
    {
        target.transform.Rotate(0, 0, 45);
        //Debug.Log(Mathf.Round(target.transform.rotation.eulerAngles.z).ToString() == target.name);
        //Debug.Log(target.transform.rotation.eulerAngles.z.ToString() == target.name);
    }

    void DropObject()
    {
        Rigidbody heldRig = heldObj.GetComponent<Rigidbody>();
        heldRig.useGravity = true;
        heldRig.drag = 1;

        heldRig.transform.parent = null;
        heldObj = null;   
    }
    void ThrowObject()
    {
        heldObj.GetComponent<Rigidbody>().AddForce(mainCam.transform.forward * throwPower); //ForceMode.impulse
        DropObject();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Teleport")) {
            // Debug.Log("TELEPORT");
            Vector3 telePos = new Vector3(teleportLocation.transform.position.x, transform.position.y, teleportLocation.transform.position.z);
            transform.position = telePos;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            gameManager.GetComponent<GameManager>().nextPuzzle();
        }
        
        else if (other.CompareTag("LockDoor")) { other.GetComponent <DoorOpen>().lockDoor(); }

    }



}
