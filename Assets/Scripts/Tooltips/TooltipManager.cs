using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Tooltip
{
    Button, Drawer, W_Flashlight, R_Flashlight, Red_door, Keycard
};

public class TooltipManager : MonoBehaviour
{

    public static TooltipManager instance;
    public static bool waitingForInput = false;

    [SerializeField] GameObject buttonTooltip;
    [SerializeField] GameObject drawerTooltip;
    [SerializeField] GameObject whiteFlashlightTooltip;
    [SerializeField] GameObject redFlashlightTooltip;
    [SerializeField] GameObject redDoorTooltip;
    [SerializeField] GameObject keycardTooltip;

    GameObject currActive;
    bool delayDone = false;

    private Dictionary<Tooltip, (GameObject, bool)> tooltips = new Dictionary<Tooltip, (GameObject, bool)>(4);

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    private void Start()
    {
        tooltips.Add(Tooltip.Button, (buttonTooltip, false));
        tooltips.Add(Tooltip.Drawer, (drawerTooltip, false));
        tooltips.Add(Tooltip.W_Flashlight, (whiteFlashlightTooltip, false));
        tooltips.Add(Tooltip.R_Flashlight, (redFlashlightTooltip, false));
        tooltips.Add(Tooltip.Red_door, (redDoorTooltip, false));
        tooltips.Add(Tooltip.Keycard, (keycardTooltip, false));
    }

    private void Update()
    {
        if (waitingForInput && Input.GetKeyDown(KeyCode.E) && delayDone)
        {
            waitingForInput = false;
            currActive.SetActive(false);
            delayDone = false;
        }
    }

    public void showTooltipObj(Tooltip t)
    {
        tooltips.TryGetValue(t, out (GameObject, bool) val);
        //Debug.Log(val.Item1.name + " " + val.Item2);

        if (!val.Item2) // tooltip has not been used before
        {
            waitingForInput = true;
            val.Item1.SetActive(true);
            currActive = val.Item1;
            tooltips[t] = (tooltips[t].Item1, true); // tooltip has not been used
            StartCoroutine(WaitTime(0.3f));
        }
    }

    private IEnumerator WaitTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        delayDone = true;
    }

}
