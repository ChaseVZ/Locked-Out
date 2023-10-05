using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject menu;
    public GameObject loadingInterface;
    public Image loadingProgressBar;

    [SerializeField] TMP_FontAsset filledFont;
    [SerializeField] TMP_FontAsset unfilledFont;

    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    int UILayer;

    bool buttonChanged = false;
    GameObject buttonChange;

    private void Start()
    {
        UILayer = LayerMask.NameToLayer("UI");
    }

    private void Update()
    {
        if (IsPointerOverUIElement())
        {
            buttonChange.GetComponent<Button>().gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().font = filledFont;
            buttonChanged = true;
        }
        else
        {
            if (buttonChanged)
            {
                buttonChange.GetComponent<Button>().gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().font = unfilledFont;
                buttonChanged = false;
            }
        }
    }

    public void PlayGame()
    {
        HideMenu();
        ShowLoadingScreen();
        scenesToLoad.Add(SceneManager.LoadSceneAsync("Gameplay"));
        scenesToLoad.Add(SceneManager.LoadSceneAsync("Puzzle01", LoadSceneMode.Additive));
        StartCoroutine(LoadingScreen());
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    IEnumerator LoadingScreen()
    {
        float totalProgress = 0;
        for (int i=0; i < scenesToLoad.Count; ++i)
        {
            while(!scenesToLoad[i].isDone)
            {
                totalProgress += scenesToLoad[i].progress;
                loadingProgressBar.fillAmount = totalProgress / scenesToLoad.Count;
                yield return null;
            }
        }
    }

    void HideMenu()
    {
        menu.SetActive(false);
    }

    void ShowLoadingScreen()
    {
        loadingInterface.SetActive(true);
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer && curRaysastResult.gameObject.tag == "ButtonUI")
            {
                unFillFont();
                buttonChange = curRaysastResult.gameObject;
                return true;
            }
        }
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    void unFillFont()
    {
        if (buttonChanged)
        {
            buttonChange.GetComponent<Button>().gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().font = unfilledFont;
            buttonChanged = false;
        }
    }
}
