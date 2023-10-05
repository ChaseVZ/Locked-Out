using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject IntroRoom;
    [SerializeField] GameObject MidGameIntroRoom;
    [SerializeField] GameObject foreverDoor;
    [SerializeField] GameObject foreverDoor2;
    [SerializeField] GameObject introDoor;

    [SerializeField] AudioSource fail1;
    [SerializeField] AudioSource success2;

    [SerializeField] Transform defaultPos;

    int twistIdx;
    int[] twistInputs = new int[3];

    int puzzleNum; /* current puzzle being played */
    bool outOfPuzzles;
    GameObject[] puzzleRooms;  /* array of all puzzle rooms */
    GameObject player;

    private void Awake()
    {
        twistIdx = 0;
        twistInputs[0] = -1;
        twistInputs[1] = -1;
        twistInputs[2] = -1;
        puzzleNum = 1;
        outOfPuzzles = false;
        player = GameObject.FindGameObjectWithTag("Player");
        
        puzzleRooms = GameObject.FindGameObjectsWithTag("PuzzleRoom"); /* used soley for gameobject.name to load scene */
        Array.Sort(puzzleRooms, CompareObNames);
    }

    public void nextPuzzle()
    {
        if (puzzleNum == 1) { loadMidGameIntroRoom(); }  /* after puzzle 1 we want to change the pre-puzzle room */

        ++puzzleNum;
        if (puzzleNum == puzzleRooms.Length+1) { outOfPuzzles = true; }
        loadNextPuzzle();

        foreverDoor.GetComponent<DoorOpen>().unlockDoor();  /* door gets locked every time, so unlock it here */
        foreverDoor2.GetComponent<DoorOpen>().unlockDoor();

        player.GetComponent<Inventory>().resetKeyCardInventory();  /* don't carry over keycards to next puzzle */
    }

    public int getPuzzleNum()
    {
        return puzzleNum;
    }

    /* setup game after player leaves intro room */
    public void loadMidGameIntroRoom()
    {
        IntroRoom.SetActive(false);
        MidGameIntroRoom.SetActive(true); 
    }

    int CompareObNames(GameObject x, GameObject y)
    {
        return x.name.CompareTo(y.name);
    }

    void loadNextPuzzle()
    {
        if (outOfPuzzles) { WinScreen();  return; }
        Debug.Log("loading: " + puzzleRooms[puzzleNum - 1].name + " and unloading: " + puzzleRooms[puzzleNum - 2].name);
        SceneManager.UnloadSceneAsync(puzzleRooms[puzzleNum - 2].name);
        SceneManager.LoadSceneAsync(puzzleRooms[puzzleNum - 1].name, LoadSceneMode.Additive);
        player.GetComponent<Flashlight>().resetReveals();
        twistIdx = 0;
    }

    void WinScreen()
    {
        UnloadAllScenes();
        SceneManager.LoadSceneAsync("WinScreen");
        Cursor.lockState = CursorLockMode.None;
    }

    void UnloadAllScenes()
    {
        int c = SceneManager.sceneCount;
        for (int i = 0; i < c; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            print(scene.name);
            SceneManager.UnloadSceneAsync(scene);
        }
    }

    public void twistEvent(int val)
    {
        if (twistIdx == 2)
        {
            twistInputs[twistIdx] = val;
            //Debug.Log("updated list: " + twistInputs[0] + " " + twistInputs[1] + " " + twistInputs[2]);

            checkLocks();
            twistIdx = 0;
        }
        else
        {
            twistInputs[twistIdx] = val;
            twistIdx++;
        }

    }

    void checkLocks()
    {
        GameObject[] cardLocks = GameObject.FindGameObjectsWithTag("CardLock");
        GameObject[] cardLocksUI = GameObject.FindGameObjectsWithTag("CardLockUI");
        var cardLocks_UI = cardLocks.Concat(cardLocksUI).ToArray();
        CardType correctColor = CardType.NONE;
        bool anyPaddedLocks = false; // if no padded locks exist here, dont do SFXs to confuse player
        bool alreadyUnlocked = false; // to ignore fail audio

        for (int i = 0; i < cardLocks.Length; i++) { 
            if (cardLocks[i].GetComponent<CardLock>().hasKeypad) { anyPaddedLocks = true; }

            // find the cardlock of interest and unpad it unpon a correct sequence
            if (cardLocks[i].GetComponent<CardLock>().confirmSequence(twistInputs)) {
                correctColor = cardLocks[i].GetComponent<CardLock>().lockType;
                if (!cardLocks[i].GetComponent<CardLock>().isPadded) { alreadyUnlocked = true; } // case where we have already unpadded it
                else { cardLocks[i].GetComponent<CardLock>().toggleOn(); }
                break; 
            }
        }

        if (!anyPaddedLocks || alreadyUnlocked) { return; }
        if (correctColor != CardType.NONE) { toggleLocks(cardLocks_UI, correctColor); }
        else { StartCoroutine(WaitAndAudio(0.4f, fail1)); }
    }

    // when a cardLock lock is solved, all other cardLocks get padded
    void toggleLocks(GameObject[] cardLocks, CardType targetColor)
    {
        for (int i = 0; i < cardLocks.Length; i++)
        {
            if (cardLocks[i].CompareTag("CardLockUI"))
            {
                cardLocks[i].GetComponent<cardlock_toggle>().checkToggle(targetColor);
            }
            else
            {
                //if (i != correctIdx) { cardLocks[i].GetComponent<CardLock>().Pad(); }
                if (cardLocks[i].GetComponent<CardLock>().lockType != targetColor) { cardLocks[i].GetComponent<CardLock>().toggleOff(); }
            }
        }
        StartCoroutine(WaitAndAudio(0.5f, success2));
    }

    private IEnumerator WaitAndAudio(float waitTime, AudioSource audio)
    {
        yield return new WaitForSeconds(waitTime);
        audio.Play();
    }

    public void resetPlayerPos()
    {
        player.transform.position = defaultPos.position;
        player.transform.rotation = Quaternion.Euler(0, 0, 0);
        foreverDoor.GetComponent<DoorOpen>().unlockDoor();  
        foreverDoor2.GetComponent<DoorOpen>().unlockDoor();
        introDoor.GetComponent<DoorOpen>().unlockDoor();
        player.GetComponent<Flashlight>().resetReveals();
    }

}
