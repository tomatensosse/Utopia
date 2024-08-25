using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("General")]
    public bool inSettingsMenu;
    public GameObject settingsMenu;
    public List<GameObject> notSettingsMenu;
    [Header("Video & Audio")]
    public GameObject videoAudioSW;
    public TMP_Dropdown resolutionDropdown;
    private List<Resolution> resolutions;
    private PlayerPrefs playerPrefs;
    [Header("Game")]
    public GameObject gameSW;
    public TMP_InputField horizontalSensInput;
    public TMP_InputField verticalSensInput;
    //TBA : CONTROLS

    [Header("Values")]
    private float horizontalSens = 2.0f;
    private float verticalSens = 2.0f;
    
    /* debug
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log(horizontalSens.ToString());
            Debug.Log(verticalSens.ToString());
        }
    }
    */

    public void Awake()
    {
        Init();
    }

    public void Init()
    {
        SetupResolutionDropdown();

        LoadPlayerPrefs();

        LoadUIFromPlayerPrefs();

        DisableSettingsMenu();
    }

    public void EnableSettingsMenu()
    {
        settingsMenu.SetActive(true);
        inSettingsMenu = true;

        foreach (GameObject obj in notSettingsMenu)
        {
            obj.SetActive(false);
        }

        ChangeTab("VideoAudio");
    }

    public void DisableSettingsMenu()
    {
        settingsMenu.SetActive(false);
        inSettingsMenu = false;

        foreach (GameObject obj in notSettingsMenu)
        {
            obj.SetActive(true);
        }

        LoadPlayerPrefs();

        LoadUIFromPlayerPrefs();
    }

    public void OnResolutionChange(int resolutionIndex)
    {
        Debug.Log("Resolution changed to " + resolutionIndex);
        Resolution resolution = Screen.resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private void SetupResolutionDropdown()
    {
        resolutions = new List<Resolution>(Screen.resolutions); // Get supported resolutions
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Count; i++)
        {
            // Round the refresh rate to the nearest integer
            int refreshRate = Mathf.RoundToInt(resolutions[i].refreshRate);

            // Create the display string with the rounded refresh rate
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + refreshRate + "Hz";
            options.Add(option);

            // Check if this resolution is currently set as the screen resolution
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height &&
                refreshRate == Screen.currentResolution.refreshRate)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(OnResolutionChange);
    }

    public void UpdateSens(string sens)
    {
        switch (sens)
        {
            case "Horizontal":
                horizontalSens = float.Parse(horizontalSensInput.text);
                break;
            case "Vertical":
                verticalSens = float.Parse(verticalSensInput.text);
                break;
        }
    }

    public void SaveButton()
    {
        SavePlayerPrefs();
        DisableSettingsMenu();
    }

    public void ChangeTab(string tab)
    {
        switch (tab)
        {
            case "VideoAudio":
                videoAudioSW.SetActive(true);
                gameSW.SetActive(false);
                break;
            case "Game":
                videoAudioSW.SetActive(false);
                gameSW.SetActive(true);
                break;
            case "Controls":
                //TBA
                break;
        }
    }

    private void LoadPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("HorizontalSens"))
        {
            horizontalSens = PlayerPrefs.GetFloat("HorizontalSens");
        }
        else
        {
            // set default
            PlayerPrefs.SetFloat("HorizontalSens", horizontalSens);
        }

        if (PlayerPrefs.HasKey("VerticalSens"))
        {
            verticalSens = PlayerPrefs.GetFloat("VerticalSens");
        }
        else
        {
            // set default
            PlayerPrefs.SetFloat("VerticalSens", verticalSens);
        }
    }

    public void SavePlayerPrefs()
    {
        PlayerPrefs.SetFloat("HorizontalSens", horizontalSens);
        PlayerPrefs.SetFloat("VerticalSens", verticalSens);

        // get local player from network manager
        if (SceneManager.GetActiveScene().name == "Game")
        {
            //LobbyController.instance.localPlayerObject.GetComponent<PlayerEntity>().LoadPlayerPrefs();
        }
    }

    private void LoadUIFromPlayerPrefs()
    {
        horizontalSensInput.text = horizontalSens.ToString();
        verticalSensInput.text = verticalSens.ToString();
    }
}
