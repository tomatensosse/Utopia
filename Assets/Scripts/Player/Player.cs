using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.SimpleWeb;
using UnityEngine;

[System.Serializable]
public class PlayerState
{
    public string playerSaveName;
    public string v_playerConnectionID;
    public string playerClass;
    public int v_playerMaxHealth; // calculate from equipment and level and shi
    public int v_playerCurrentHealth;
    public int playerScore;

    public void LoadFromSave(PlayerSave playerSave)
    {
        // only load the data WITHOUT v_ prefix
        // because v_ prefix will be calculated or set in each session
        playerSaveName = playerSave.playerSaveName;
        playerClass = playerSave._demoPlayerClass;
        playerScore = playerSave.playerScore;
    }

    public PlayerSave ToSave()
    {
        // only save the data WITHOUT v_ prefix
        // because v_ prefix will be calculated or set in each session
        PlayerSave playerSave = new PlayerSave();
        playerSave.playerSaveName = playerSaveName;
        playerSave._demoPlayerClass = playerClass;
        playerSave.playerScore = playerScore;
        return playerSave;
    }

    public void PrintDebug()
    {
        Debug.Log("PlayerSaveName: " + playerSaveName);
        Debug.Log("PlayerClass: " + playerClass);
        Debug.Log("PlayerScore: " + playerScore);
    }
}

public class Player : NetworkBehaviour
{

    [SyncVar] public bool isLoadedOrInitialized;
    [SyncVar] public string connectionID;
    public PlayerState playerState = new PlayerState();

    [Header("Player Scripts")]
    public PlayerMovement playerMovement;
    public InventoryManager playerInventoryManager;
    public Inventory playerInventory;

    public void Start()
    {
        // make sure it's not destroyed when we change scenes (FIX LATER)
        DontDestroyOnLoad(gameObject);
    }

    public void InitializePlayer(PlayerSave playerSave, string playerConnectionID)
    {
        this.playerState.LoadFromSave(playerSave);
        this.playerState.v_playerConnectionID = playerConnectionID;
        Debug.Log($"[Server] Initializing player with connection ID: {playerConnectionID} and save data.");

        ItemData[] inventoryToSet = playerSave.inventory;
        playerInventory.SetInventory(inventoryToSet);

        isLoadedOrInitialized = true;
    }

    public void LoadExistingPlayer(PlayerState playerState)
    {
        this.playerState = playerState;

        isLoadedOrInitialized = true;
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log($"[Client] Player initialized on client with ID: {connectionID}");

        if (isLocalPlayer)
        {
            // Additional local player initialization if needed
            playerMovement.inputEnabled = true;
        }

        // Initialize ui, camera, etc.
        playerMovement.InitializePlayer();
        playerInventoryManager.Initialize();
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                playerMovement.inputEnabled = !playerMovement.inputEnabled;
                GameUI.Instance.ToggleTabs();
            }

            if (Input.inputString != null)
            {
                playerInventoryManager.HotbarSwap(Input.inputString);
            }
        }
    }

    private void OnApplicationQuit() // or Disconnect
    {
        if (isLocalPlayer)
        {
            // Save player state
            PlayerSave playerNewSave = playerState.ToSave();

            // Reference SaveSystem and Save Player
        }
    }
}
