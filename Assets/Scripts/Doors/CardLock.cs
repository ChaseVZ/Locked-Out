using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLock : MonoBehaviour
{

    [SerializeField] public CardType lockType;
    [SerializeField] GameObject card;
    [SerializeField] GameObject keypad;
    [SerializeField] public bool hasKeypad;
    [SerializeField] bool beginPadded;
    [SerializeField] bool beginCarded;
    [SerializeField] int[] sequence;

    [SerializeField] GameObject _light;
    [SerializeField] GameObject deactive;
    public bool isPadded;

    bool isCard;
    bool justTookCard; /* took card out and hasnt left range of door yet */
    bool removeCardonUnPad; /* we always want a card on padded locks, but dont give it to user if they didnt place it */
    GameObject player;
    AudioSource itemGrab;
    AudioSource placeCardSFX;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        placeCardSFX = GetComponent<AudioSource>();
        itemGrab = GameObject.Find("ItemGrab").GetComponent<AudioSource>();
        isCard = false;
        justTookCard = false;
        removeCardonUnPad = true;

        if (beginCarded) { placeCard(); }
        keypad.SetActive(false);
        
        if (beginPadded) { toggleOff(); }
        else {
            toggleOn();
            //keypad.SetActive(false); isPadded = false; 
        }
    }

    public void toggleOn()
    {
        if (hasKeypad)
        {
            _light.SetActive(true);
            deactive.SetActive(false);
            isPadded = false;
        }
    }

    public void toggleOff()
    {
        if (hasKeypad)
        {
            Debug.Log("turning off: " + this.name);
            _light.SetActive(false);
            deactive.SetActive(true);
            isPadded = true;
        }
        //Pad();

    }

    /* fails if player doesnt have card needed OR this lock is already filled JK thats true too now */
    public bool TryInsert()
    {
        if(!isCard && setInventory(false))
        {
            placeCardSFX.Play();
            placeCard();
            return true;
        }
        else if (isCard) { return true; }
        return false;
    }

    void placeCard()
    {
        isCard = true;
        card.SetActive(true);
    }

    public bool hasCard() 
    {
        if (hasKeypad && isPadded) { return true; } // currently locked, padded cardLocks are considered to 'have card'
        return isCard;
    }

    public void unPad()
    {
        if (hasKeypad)
        {
            isPadded = false;
            keypad.SetActive(false);

            if (removeCardonUnPad) { card.SetActive(false); isCard = false; }
        }
    }

    // place keycard inside if it doesnt already have one so player isnt confused
    public void Pad()
    {
        if (hasKeypad)
        {
            isPadded = true;
            keypad.SetActive(true);

            if (isCard) { removeCardonUnPad = false; }
            else { removeCardonUnPad = true; }

            card.SetActive(true);
        }
    }


    public bool confirmSequence(int[] inputSeq)
    {
        // Debug.Log("for: " + name + "'s input: " + inputSeq[0] + " " + inputSeq[1] + " " + inputSeq[2] + " to mine: " + sequence[0] + " " + sequence[1] + " " + sequence[2]);
        if (inputSeq.Length == sequence.Length && CheckMatch(inputSeq, sequence)) { 
            //if (isPadded) { unPad(); }
            return true;  // return true even if unpadded; check isPadded in game manager so we can avoid fail audio
        }
        return false;
    }

    bool CheckMatch(int[] l1, int[] l2)
    {
        if (l1.Length != l2.Length)
            return false;
        for (int i = 0; i < l1.Length; i++)
        {
            if (l1[i] != l2[i])
                return false;
        }
        return true;
    }

    public CardType whatType() { return lockType; }

    public void TakeorPlaceCard() 
    {
        if (isCard)
        {
            isCard = false;
            card.SetActive(false);

            setInventory(true);
            itemGrab.Play();
            justTookCard = true;
        }
        else
        {
            TryInsert();
            justTookCard = false;
        }
    }

    public void setJustTookCard(bool setter) { justTookCard = setter; }
    public bool getJustTookCard() { return justTookCard; }

    /* returns true if set set/took card */
    /* if flag = FALSE: checking if player has it in inventory, taking it, and setting inventory to false */
    /* if flag = TRUE: player is taking card back, set appropriate inventory place to true */
    /* "|| flag" has been added to allow for 2+ cards to be obtained */
    bool setInventory(bool flag)
    {
        switch (lockType)
        {
            case CardType.Black:
                if (player.GetComponent<Inventory>().getBlack() == !flag || flag) { player.GetComponent<Inventory>().setBlack(flag); return true; }
                return false;
            case CardType.Blue:
                if (player.GetComponent<Inventory>().getBlue() == !flag || flag) { player.GetComponent<Inventory>().setBlue(flag); return true; }
                return false;
            case CardType.Green:
                if (player.GetComponent<Inventory>().getGreen() == !flag || flag) { player.GetComponent<Inventory>().setGreen(flag); return true; }
                return false; ;
            case CardType.Red:
                if (player.GetComponent<Inventory>().getRed() == !flag || flag) { player.GetComponent<Inventory>().setRed(flag); return true; }
                return false;
            case CardType.Yellow:
                if (player.GetComponent<Inventory>().getYellow() == !flag || flag) { player.GetComponent<Inventory>().setYellow(flag); return true; }
                return false;
            default:
                Debug.Log("card error");
                return false;
        }
    }
}

public enum CardType
{
    Black, Blue, Green, Red, Yellow, NONE
};
