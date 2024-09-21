using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }

    [Header("UI")]
    private string currentSelection;

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
                selectionUI.SetActive(true);
                playerCreationUI.SetActive(false);
                worldCreationUI.SetActive(false);
                joinSelectionUI.SetActive(false);
                titleScreenUI.SetActive(false);
                settingsUI.SetActive(false);
                break;
            case "PlayerCreationUI":
                selectionUI.SetActive(false);
                playerCreationUI.SetActive(true);
                worldCreationUI.SetActive(false);
                joinSelectionUI.SetActive(false);
                titleScreenUI.SetActive(false);
                settingsUI.SetActive(false);
                break;
            case "WorldCreationUI":
                selectionUI.SetActive(false);
                playerCreationUI.SetActive(false);
                worldCreationUI.SetActive(true);
                joinSelectionUI.SetActive(false);
                titleScreenUI.SetActive(false);
                settingsUI.SetActive(false);
                break;
            case "JoinSelectionUI":
                selectionUI.SetActive(false);
                playerCreationUI.SetActive(false);
                worldCreationUI.SetActive(false);
                joinSelectionUI.SetActive(true);
                titleScreenUI.SetActive(false);
                settingsUI.SetActive(false);
                break;
            case "TitleScreenUI":
                selectionUI.SetActive(false);
                playerCreationUI.SetActive(false);
                worldCreationUI.SetActive(false);
                joinSelectionUI.SetActive(false);
                titleScreenUI.SetActive(true);
                settingsUI.SetActive(false);
                break;
            case "SettingsUI":
                selectionUI.SetActive(false);
                playerCreationUI.SetActive(false);
                worldCreationUI.SetActive(false);
                joinSelectionUI.SetActive(false);
                titleScreenUI.SetActive(false);
                settingsUI.SetActive(true);
                break;
        }
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
        PlayerData playerSave = new PlayerData();
        playerSave.playerSaveName = playerNameInput.text;
        playerSave.playerClass = playerClassInput.text;
        saveSystem.SavePlayer(playerSave, playerSave.playerSaveName);
        ChangeTo("SelectionUI");
        LoadSelectionMenuContent();
    }

    public void CreateWorld()
    {
        WorldData worldSave = new WorldData();
        worldSave.worldSaveName = worldNameInput.text;
        worldSave.worldSeed = int.Parse(worldSeedInput.text);
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
