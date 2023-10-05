using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    int[] hasCard;
    const int black = 0;
    const int blue = 1;
    const int green = 2;
    const int red = 3;
    const int yellow = 4;

    bool hasWhiteFlashlight;
    bool hasRedFlashlight;

    [SerializeField] Sprite blueImage;
    [SerializeField] Sprite blackImage;
    [SerializeField] Sprite redImage;
    [SerializeField] Sprite greenImage;
    [SerializeField] Sprite yellowImage;

    [SerializeField] GameObject inventory1;
    [SerializeField] GameObject inventory2;
    [SerializeField] GameObject inventory3;
    [SerializeField] GameObject inventory4;
    [SerializeField] GameObject inventory5;

    [SerializeField] GameObject flashlightWhite;
    [SerializeField] GameObject flashlightRed;

    public bool gotWhiteFlashlightThisLevel = false;
    public bool gotRedFlashlightThisLevel = false;

    List<GameObject> keycardInventory;

    //int numCards = 0;

    void Start()
    {
        hasCard = new int[] { 0, 0, 0, 0, 0 };

        hasWhiteFlashlight = false;
        hasRedFlashlight = false;

        keycardInventory = new List<GameObject> { inventory1, inventory2, inventory3, inventory4, inventory5 };
    }

    #region Setters
    public void setBlack(bool setter)
    {
        hasCard[black] += setter ? 1 : -1;
        updateKeyCardUI();
    }

    public void setBlue(bool setter)
    {
        hasCard[blue] += setter ? 1 : -1;
        updateKeyCardUI();
    }

    public void setGreen(bool setter)
    {
        hasCard[green] += setter ? 1 : -1;
        updateKeyCardUI();
    }

    public void setRed(bool setter)
    {
        hasCard[red] += setter ? 1 : -1;
        updateKeyCardUI();
    }

    public void setYellow(bool setter)
    {
        hasCard[yellow] += (setter ? 1 : -1);
        updateKeyCardUI();
    }

    public void setWhiteFlashlight(bool setter) { hasWhiteFlashlight = setter; if (setter) { flashlightWhite.SetActive(true); gotWhiteFlashlightThisLevel = true; } }
    public void setRedFlashlight(bool setter) { hasRedFlashlight = setter; if (setter) { flashlightRed.SetActive(true); gotRedFlashlightThisLevel = true; } }
    #endregion


    #region Getters
    public bool getBlack()
    {
        return hasCard[black] > 0;
    }

    public bool getBlue()
    {
        return hasCard[blue] > 0;
    }

    public bool getGreen()
    {
        return hasCard[green] > 0;
    }

    public bool getRed()
    {
        return hasCard[red] > 0;
    }

    public bool getYellow()
    {
        return hasCard[yellow] > 0;
    }

    public bool getWhiteFlashlight() { return hasWhiteFlashlight; }
    public bool getRedFlashlight() { return hasRedFlashlight; }
    #endregion

    public void resetKeyCardInventory()
    {
        hasCard[black] = 0;
        hasCard[blue] = 0;
        hasCard[green] = 0;
        hasCard[red] = 0;
        hasCard[yellow] = 0;
        clearKeyCardInventoryUI();
    }

    public void resetFlashlightInventory()
    {
        hasWhiteFlashlight = false;
        hasRedFlashlight = false;
        flashlightWhite.SetActive(false);
        flashlightRed.SetActive(false);
    }

    public bool hasCardOfType(CardType ct)
    {
        switch (ct)
        {
            case CardType.Black:
                return getBlack();
            case CardType.Blue:
                return getBlue();
            case CardType.Green:
                return getGreen();
            case CardType.Red:
                return getRed();
            case CardType.Yellow:
                return getYellow();
            default:
                return false;
        }
    } 

    void updateKeyCardUI()
    {
        clearKeyCardInventoryUI();
        setKeyCardInventoryUI();
    }

    void setKeyCardInventoryUI()
    {
        int idx = 0;

        if (getBlack())
        {
            keycardInventory[idx].SetActive(true);
            keycardInventory[idx].GetComponent<Image>().sprite = blackImage;
            keycardInventory[idx].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = hasCard[black].ToString();
            idx++;
        }
        if (getBlue())
        {
            keycardInventory[idx].SetActive(true);
            keycardInventory[idx].GetComponent<Image>().sprite = blueImage;
            keycardInventory[idx].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = hasCard[blue].ToString();
            idx++;
        }
        if (getRed())
        {
            keycardInventory[idx].SetActive(true);
            keycardInventory[idx].GetComponent<Image>().sprite = redImage;
            keycardInventory[idx].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = hasCard[red].ToString();
            idx++;
        }
        if (getGreen())
        {
            keycardInventory[idx].SetActive(true);
            keycardInventory[idx].GetComponent<Image>().sprite = greenImage;
            keycardInventory[idx].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = hasCard[green].ToString();
            idx++;
        }
        if (getYellow())
        {
            keycardInventory[idx].SetActive(true);
            keycardInventory[idx].GetComponent<Image>().sprite = yellowImage;
            keycardInventory[idx].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = hasCard[yellow].ToString();
            idx++;
        }
    }


    void clearKeyCardInventoryUI()
    {
        foreach (GameObject spot in keycardInventory)
        {
            spot.SetActive(false);
        }
    }

    public void levelReset()
    {
        clearKeyCardInventoryUI();
        resetKeyCardInventory();

        // reset flashlight if they just got it
        if (gotWhiteFlashlightThisLevel) { hasWhiteFlashlight = false; flashlightWhite.SetActive(false);  gotWhiteFlashlightThisLevel = false; }
        if (gotRedFlashlightThisLevel) { hasRedFlashlight = false; flashlightRed.SetActive(false); gotWhiteFlashlightThisLevel = false; }

        flashlightWhite.SetActive(false);
        flashlightRed.SetActive(false);
    }

    public void checkGrabbedLight()
    {
        if (gotWhiteFlashlightThisLevel) { gotWhiteFlashlightThisLevel = false; }
        if (gotRedFlashlightThisLevel) { gotRedFlashlightThisLevel = false; }
    }
}
