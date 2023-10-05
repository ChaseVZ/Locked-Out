using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;
    public GameObject keybindsMenuUI;

    GameObject player;
    GameObject gameManager;

    float latestScrollVal;

    [SerializeField] Scrollbar mouseSensScroll;
    [SerializeField] TMP_InputField mouseSenseIF;

    [SerializeField] Canvas crosshair;

    [SerializeField] TMP_FontAsset filledFont;
    [SerializeField] TMP_FontAsset unfilledFont;

    int UILayer;

    bool buttonChanged = false;
    GameObject buttonChange;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = GameObject.FindGameObjectWithTag("GameManager");

        mouseSenseIF.text = getCurrPlayerSens().ToString();

        latestScrollVal = getLerpValue(getCurrPlayerSens());
        mouseSensScroll.value = getCurrPlayerSens() / 2.0f;

        UILayer = LayerMask.NameToLayer("UI");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }else
            {
                Pause();
            }
        }

        if (!GameIsPaused) { return; }

        float newScrollSens = getLerpValue(mouseSensScroll.value);

        if (latestScrollVal != newScrollSens)
        {
            setSensVals(newScrollSens);
        }

        if (mouseSenseIF.isFocused && mouseSenseIF.text != "" && mouseSenseIF.text != ".")
        {
            float newSens = float.Parse(mouseSenseIF.text);
            if (newSens > 2.0f) { newSens = 2.0f; mouseSenseIF.text = "2.0"; }  

            player.GetComponent<FirstPersonController>().mouseSensitivity = newSens;
            mouseSensScroll.value = newSens / 2.0f;
            latestScrollVal = newSens;
        }
        if (!mouseSenseIF.isFocused && (mouseSenseIF.text == "" || mouseSenseIF.text == "."))
        {
            mouseSenseIF.text = player.GetComponent<FirstPersonController>().mouseSensitivity.ToString();
        }


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

    void setSensVals(float newSens)
    {
        latestScrollVal = newSens;

        mouseSenseIF.text = (Mathf.Round(newSens * 100f) / 100f).ToString();
        mouseSensScroll.value = newSens/2.0f;

        player.GetComponent<FirstPersonController>().mouseSensitivity = Mathf.Lerp(0.0f, 2.0f, newSens);
    }

    float getLerpValue(float t)
    {
        return Mathf.Lerp(0.0f, 2.0f, t);
    }

    float getLerpValue0_1(float t)
    {
        return Mathf.Lerp(0.0f, 2.0f, t);
    }

    float getCurrPlayerSens()
    {
        return player.GetComponent<FirstPersonController>().mouseSensitivity;
    }


    void Pause()
    {
        player.GetComponent<FirstPersonController>().cameraCanMove = false;
        Cursor.lockState = CursorLockMode.None;
        pauseMenuUI.SetActive(true);
        crosshair.gameObject.SetActive(false);
        Time.timeScale = 0f;    /* Pause time */
        GameIsPaused = true;
    }

    public void Resume()
    {
        player.GetComponent<FirstPersonController>().cameraCanMove = true;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        keybindsMenuUI.SetActive(false);
        crosshair.gameObject.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }

    public void RestartLevel()
    {
        Debug.Log("Restarting Level");
        restartActiveScenes();
        resetPlayer();
        Resume();
    }

    void restartActiveScenes()
    {
        int countLoaded = SceneManager.sceneCount;
        //Scene[] loadedScenes = new Scene[countLoaded];

        for (int i = 0; i < countLoaded; i++)
        {
            Debug.Log(SceneManager.GetSceneAt(i).name);

            if (SceneManager.GetSceneAt(i).name == "Gameplay") { } //SceneManager.LoadScene("Gameplay"); }
            else {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i).name);
                SceneManager.LoadScene(SceneManager.GetSceneAt(i).name, LoadSceneMode.Additive); 
            }
        }
    }

    void resetPlayer()
    {
        gameManager.GetComponent<GameManager>().resetPlayerPos();
        player.GetComponent<Inventory>().levelReset();
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
