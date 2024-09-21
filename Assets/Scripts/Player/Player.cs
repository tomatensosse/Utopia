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

    public void LoadFromSave(PlayerData playerSave)
    {
        // only load the data WITHOUT v_ prefix
        // because v_ prefix will be calculated or set in each session
        playerSaveName = playerSave.playerSaveName;
        playerClass = playerSave.playerClass;
        playerScore = playerSave.playerScore;
    }

    public PlayerData ToSave()
    {
        // only save the data WITHOUT v_ prefix
        // because v_ prefix will be calculated or set in each session
        PlayerData playerSave = new PlayerData();
        playerSave.playerSaveName = playerSaveName;
        playerSave.playerClass = playerClass;
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

public class Player : NetworkBehaviour, IHealth
{

    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }

    [SyncVar] public bool isLoadedOrInitialized;
    [SyncVar] public string connectionID;
    public PlayerState playerState = new PlayerState();

    [Header("Player Scripts")]
    public PlayerController playerMovement;
    public Inventory inventory;

    public void Start()
    {
        // make sure it's not destroyed when we change scenes (FIX LATER)
        DontDestroyOnLoad(gameObject);
    }

    public void InitializePlayer(PlayerData playerSave, string playerConnectionID)
    {
        this.playerState.LoadFromSave(playerSave);
        this.playerState.v_playerConnectionID = playerConnectionID;
        Debug.Log($"[Server] Initializing player with connection ID: {playerConnectionID} and save data.");

        MaxHealth = 100; // Calculate health from equipment, level, etc.
        CurrentHealth = MaxHealth;

        isLoadedOrInitialized = true;
    }

    public void LoadExistingPlayer(PlayerState playerState)
    {
        this.playerState = playerState;

        MaxHealth = playerState.v_playerMaxHealth;
        CurrentHealth = playerState.v_playerCurrentHealth;

        isLoadedOrInitialized = true;
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log($"[Client] Player initialized on client with ID: {connectionID}");

        if (isLocalPlayer)
        {
            // Additional local player initialization if needed
            playerMovement.inputEnabled = true;
            InventoryManager.Instance.inventory = inventory;
            inventory.Initialize();
        }

        // Initialize ui, camera, etc.
        playerMovement.InitializePlayer();

        GameManagerUI.Instance.Init();
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            // Handle input, movement, etc.
            // Update playerState as needed
        }
    }

    [Command (requiresAuthority = false)]
    public void CmdTakeDamage(int damage, NetworkConnectionToClient conn)
    {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Die(conn);
        }

        RpcUpdateHealth(CurrentHealth);
    }

    [ClientRpc]
    public void RpcUpdateHealth(int health)
    {
        CurrentHealth = health;
        // Update UI or show other visual indicators
    }

    public bool IsDead()
    {
        return CurrentHealth <= 0;
    }

    public void Die(NetworkConnectionToClient conn)
    {
        // Handle player death
        Debug.Log(conn.identity + " has died.");
        // Respawn, show death screen, etc.
    }

    private void OnApplicationQuit() // or Disconnect
    {
        if (isLocalPlayer)
        {
            // Save player state
            PlayerData playerNewSave = playerState.ToSave();

            // Reference SaveSystem and Save Player
        }
    }
}
