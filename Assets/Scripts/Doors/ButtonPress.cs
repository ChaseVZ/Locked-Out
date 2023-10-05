using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{

    //[SerializeField] GameObject door;
    [SerializeField] GameObject barrier;
    [SerializeField] GameObject drawer;
    [SerializeField] bool hasDrawer;
    [SerializeField] bool StartBlocked;
    [SerializeField] DoorColor buttonColor;

    AudioSource UIClick;
    AudioSource unlockAudio;
    GameObject[] conditions;
    List<GameObject> doors = new List<GameObject>();
    [SerializeField] bool hasConditions;
    [SerializeField] string conditionString;

    bool blocked;
    Animator animator;
    int isPressed;
    int isOpen;

    private void Start()
    {
        AudioSource[] audios = GetComponents<AudioSource>();
        UIClick = audios[0];
        unlockAudio = audios[1];

        animator = GetComponent<Animator>();
        if (hasConditions) { conditions = GameObject.FindGameObjectsWithTag(conditionString); }
        
        isPressed = Animator.StringToHash("Pressed");
        isOpen = Animator.StringToHash("Open");

        GameObject[] allDoors = GameObject.FindGameObjectsWithTag("ButtonDoor");
        foreach (GameObject d in allDoors)
        {
            if (d.GetComponent<DoorOpen>().doorC == buttonColor) { doors.Add(d); }
        }

        blocked = StartBlocked;

        if (StartBlocked) { barrier.SetActive(true); }
        //else { barrier.SetActive(false); }
    }

    private void Update()
    {
        if (StartBlocked && blocked)
        {
            if(hasConditions && passConditions())
            {
                unlockAudio.Play();
                blocked = false;
                barrier.SetActive(false);
            }
        }
    }

    public void interact()
    {
        if (!blocked)
        {
            if (!animator.GetBool(isPressed))
            {
                animator.SetBool(isPressed, true);
                //bool doorOpened = door.GetComponent<DoorOpen>().ButtonPressed();
                foreach (GameObject d in doors)
                {
                    d.GetComponent<DoorOpen>().ButtonPressed();
                }

                if (hasDrawer && !drawer.GetComponent<Animator>().GetBool(isOpen))
                {
                    Debug.Log("open Drawer");
                    drawer.GetComponent<Animator>().SetBool(isOpen, true);
                }

                UIClick.Play(); 
            }
                
        }
    }

    bool passConditions()
    {
        foreach (GameObject cond in conditions)
        {
            if (Mathf.Round(cond.transform.rotation.eulerAngles.z).ToString() != cond.name &&
                Mathf.Round(cond.transform.rotation.eulerAngles.z + 360).ToString() != cond.name &&
                Mathf.Round(cond.transform.rotation.eulerAngles.z - 360).ToString() != cond.name) { return false; }

        }
        return true;
    }

}
