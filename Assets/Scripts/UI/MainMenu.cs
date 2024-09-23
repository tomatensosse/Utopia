using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }

    [Header("UI")]
    private string currentSelection;

    public GameObject[] allSelectionUI;

    public GameObject selectionUI;

    public Transform playerSavesContent;

    public Transform worldSavesContent;

    public GameObject playerCreationUI;

    public GameObject worldCreationUI;

    public GameObject joinSelectionUI;

    public Transform joinSelectionContent;

    public GameObject titleScreenUI;

    public GameObject settingsUI;

    [Header("Player Creation")]
    public TMP_InputField playerNameInput;

    public TMP_InputField playerClassInput;

    [Header("World Creation")]
    public TMP_InputField worldNameInput;

    public TMP_InputField worldSeedInput;

    [Header("Save Management")]
    public GameObject selectionMenuContentPrefab;

    private SaveSystem saveSystem = new SaveSystem();
    
    private CustomNetworkManager networkManager;
    public ErrorPopup errorPopup; 
    public TMP_Dropdown characterClassSelectDropdown; // Assign in the Inspector

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        networkManager = FindObjectOfType<CustomNetworkManager>();
    }

    public void Start()
    {
        ChangeTo("TitleScreenUI");
        LoadSelectionMenuContent();
    }

    public void ChangeTo(string selection)
    {
        currentSelection = selection;

        switch (selection)
        {
            case "SelectionUI":
                DeactivateAllUI();
                selectionUI.SetActive(true);
                break;
            case "PlayerCreationUI":
                DeactivateAllUI();
                playerCreationUI.SetActive(true);
                break;
            case "WorldCreationUI":
                DeactivateAllUI();
                worldCreationUI.SetActive(true);
                break;
            case "JoinSelectionUI":
                DeactivateAllUI();
                joinSelectionUI.SetActive(true);
                break;
            case "TitleScreenUI":
                DeactivateAllUI();
                titleScreenUI.SetActive(true);
                break;
            case "SettingsUI":
                DeactivateAllUI();
                settingsUI.SetActive(true);
                break;
        }
    }
    
    public void DeactivateAllUI()
    {
        // LIST ALL UI HERE!!!!
        allSelectionUI = new GameObject[] {
            selectionUI,
            playerCreationUI,
            worldCreationUI,
            joinSelectionUI,
            titleScreenUI,
            settingsUI
        };

        foreach (GameObject ui in allSelectionUI)
        {
            if (ui != null) {
                ui.SetActive(false);
            }
        };

    }

    public void LoadSelectionMenuContent()
    {
        if (currentSelection != "JoinSelectionUI")
        {
            foreach (Transform child in playerSavesContent)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in worldSavesContent)
            {
                Destroy(child.gameObject);
            }

            foreach (PlayerData playerSave in saveSystem.LoadAllPlayers())
            {
                GameObject selectionMenuContent = Instantiate(selectionMenuContentPrefab, selectionUI.transform);
                selectionMenuContent.transform.SetParent(playerSavesContent);
                SelectionMenuContent selectionMenuContentScript = selectionMenuContent.GetComponent<SelectionMenuContent>();
                selectionMenuContentScript.saveData = playerSave;
                selectionMenuContentScript.displayText.text = playerSave.playerSaveName;
            }

            foreach (WorldData worldSave in saveSystem.LoadAllWorlds())
            {
                GameObject selectionMenuContent = Instantiate(selectionMenuContentPrefab, selectionUI.transform);
                selectionMenuContent.transform.SetParent(worldSavesContent);
                SelectionMenuContent selectionMenuContentScript = selectionMenuContent.GetComponent<SelectionMenuContent>();
                selectionMenuContentScript.saveData = worldSave;
                selectionMenuContentScript.displayText.text = worldSave.worldSaveName;
            }

            return;
        }
        else
        {
            foreach (Transform child in joinSelectionContent)
            {
                Destroy(child.gameObject);
            }

            foreach (PlayerData playerSave in saveSystem.LoadAllPlayers())
            {
                GameObject selectionMenuContent = Instantiate(selectionMenuContentPrefab, selectionUI.transform);
                selectionMenuContent.transform.SetParent(joinSelectionContent);
                SelectionMenuContent selectionMenuContentScript = selectionMenuContent.GetComponent<SelectionMenuContent>();
                selectionMenuContentScript.saveData = playerSave;
                selectionMenuContentScript.displayText.text = playerSave.playerSaveName;
            }
        }
    }

    public void CreatePlayer()
    {
        string inputName = playerNameInput.text;
        string fullInputClass = characterClassSelectDropdown.options[characterClassSelectDropdown.value].text;
        string inputClass = fullInputClass.Split(':')[0];
        Debug.Log(inputClass);

        string validNameRegex = "^[a-zA-Z0-9._-]{1,20}$";

        if (validateRegex(inputName,validNameRegex) != true) { // Validate Name
            errorPopup.ShowError("Name should be 1-20 letters, include only a-z, A-Z, and . _ -", 4);
            return;
        }

        string[] playerNames = saveSystem.LoadAllPlayers().Select(ply => ply.playerSaveName).ToArray();
        
        if (playerNames.Contains(inputName) == true) {
            errorPopup.ShowError("The name \"" + inputName + "\" is already taken!", 4);
            return;
        }

        if (inputClass == "Choose Class") {
            errorPopup.ShowError("Please choose a class!", 4);
            return;
        }   

        PlayerData playerSave = new PlayerData();
        playerSave.playerSaveName = inputName;
        playerSave.playerClass = inputClass;
        saveSystem.SavePlayer(playerSave, inputName);
        ChangeTo("SelectionUI");
        LoadSelectionMenuContent();
    }

    public bool validateRegex(string nameToCheck, string RegexToCheck) 
    {
        return Regex.IsMatch(nameToCheck, RegexToCheck);
    }


    public void CreateWorld()
    {
        WorldData worldSave = new WorldData();
        worldSave.worldSaveName = worldNameInput.text;
        worldSave.worldSeed = int.Parse(worldSeedInput.text);
        worldSave.chunkDatas = new List<ChunkData>(); // To World Size
        saveSystem.SaveWorld(worldSave, worldSave.worldSaveName);
        ChangeTo("SelectionUI");
        LoadSelectionMenuContent();
    }

    public void ResetPlayerButtons()
    {
        foreach (Transform child in playerSavesContent)
        {
            SelectionMenuContent selectionMenuContentScript = child.GetComponent<SelectionMenuContent>();
            selectionMenuContentScript.ResetButton();
        }
    }

    public void ResetWorldButtons()
    {
        foreach (Transform child in worldSavesContent)
        {
            SelectionMenuContent selectionMenuContentScript = child.GetComponent<SelectionMenuContent>();
            selectionMenuContentScript.ResetButton();
        }
    }

    public void HostGame()
    {
        networkManager.StartGameAsHost(GameManager.Instance.setPlayer, GameManager.Instance.setWorld);
    }

    public void OnClientConnected()
    {
        ChangeTo("JoinSelectionUI");
        LoadSelectionMenuContent();
    }

    public void FinalizeCharacterSelection()
    {
        networkManager.FinalizeClientPlayer(GameManager.Instance.setPlayer);
    }
}
