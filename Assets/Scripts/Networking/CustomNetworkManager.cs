using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct PlayerSaveMessage : NetworkMessage
{
    public PlayerSave playerSave;
    public string connectionID;
}

public struct WorldSaveMessage : NetworkMessage
{
    public WorldSave worldSave;
}

public struct PlayerStateMessage : NetworkMessage
{
    public List<PlayerState> playerStates;
}

public class CustomNetworkManager : NetworkManager
{
    [Header("Custom Settings")]
    public PlayerSave hostPlayerSave;
    public PlayerSave clientPlayerSave;
    public WorldSave hostWorldSave;
    private bool gameSceneLoaded = false;

    public override void OnStartHost()
    {
        base.OnStartHost();
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Create Steam Lobby if needed
        SteamLobby steamLobby = FindObjectOfType<SteamLobby>();
        if (steamLobby != null)
        {
            steamLobby.CreateLobby();
        }

        NetworkServer.RegisterHandler<PlayerSaveMessage>(OnPlayerSaveMessageReceived);

        // Load the world for the host
        LoadWorld(hostWorldSave);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("[Client] Registering handlers for PlayerSaveMessage and PlayerStateMessage.");
        NetworkClient.RegisterHandler<PlayerSaveMessage>(OnClientPlayerSaveMessageRecieved);
        NetworkClient.RegisterHandler<PlayerStateMessage>(OnPlayerStateMessageReceived);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game") // Ensure this is your game scene's name
        {
            gameSceneLoaded = true;

            if (NetworkServer.active)
            {
                Debug.Log("Host loaded game scene");
                // Server: Spawn objects for existing connections
                NetworkServer.SpawnObjects();
                OnServerSceneLoadedForPlayers();
            }

            if (NetworkClient.active && !NetworkServer.active)
            {
                Debug.Log("Client loaded game scene");
                // Client: Handle client-specific player initialization
                OnClientSceneLoadedForPlayers();
            }
        }
    }

    private void OnServerSceneLoadedForPlayers()
    {
        GameObject hostPlayer = Instantiate(playerPrefab);
        Player hostPlayerComponent = hostPlayer.GetComponent<Player>();

        hostPlayerComponent.InitializePlayer(hostPlayerSave, "HOST");
        hostPlayerComponent.connectionID = "HOST";

        NetworkServer.AddPlayerForConnection(NetworkServer.localConnection, hostPlayer);
    }

    private void OnClientSceneLoadedForPlayers()
    {
        Debug.Log("[Client] Client scene loaded.");

        if (!NetworkClient.ready)
        {
            NetworkClient.Ready();
        }

        Debug.Log("Client is " + (NetworkClient.ready ? "ready" : "not ready"));

        NetworkClient.Send(new PlayerSaveMessage { playerSave = clientPlayerSave, connectionID = NetworkClient.connection.connectionId.ToString() });
    }

    private void OnPlayerSaveMessageReceived(NetworkConnectionToClient conn, PlayerSaveMessage message)
    {
        Debug.Log("[Server] Received player save message.");

        GameObject playerGameObject = Instantiate(playerPrefab);
        Player player = playerGameObject.GetComponent<Player>();
        player.InitializePlayer(message.playerSave, conn.connectionId.ToString());
        player.connectionID = conn.connectionId.ToString();
        
        NetworkServer.AddPlayerForConnection(conn, playerGameObject);
        NetworkServer.Spawn(playerGameObject);

        NetworkServer.SendToAll(new PlayerSaveMessage { playerSave = message.playerSave, connectionID = conn.connectionId.ToString() });

        // Send every player to the client
        List<PlayerState> playerStates = new List<PlayerState>();
        foreach (var connection in NetworkServer.connections)
        {
            if (conn.identity.connectionToClient != connection.Value.identity.connectionToClient)
            {
                Player sendPlayer = connection.Value.identity.GetComponent<Player>();
                playerStates.Add(sendPlayer.playerState);
            }
        }

        PlayerStateMessage playerStateMessage = new PlayerStateMessage { playerStates = playerStates };

        Debug.Log("[Server] Sending PlayerStateMessage to the new client.");
        conn.Send(playerStateMessage);

        // Send the world save to the player
        //conn.Send(new WorldSaveMessage { worldSave = GameManager.Instance.worldSave });
    } 

    private void OnClientPlayerSaveMessageRecieved(PlayerSaveMessage message)
    {
        Debug.Log("[Client] Received player save message.");

        StartCoroutine(InitializeClientPlayer(message));
    }

    private void OnPlayerStateMessageReceived(PlayerStateMessage message)
    {
        foreach (var playerState in message.playerStates)
        {
            Player player = GetPlayerByConnectionID(playerState.v_playerConnectionID);
            if (player != null)
            {
                player.LoadExistingPlayer(playerState);
                
                Debug.Log("Player loaded: " + playerState.playerSaveName);
            }
            else
            {
                Debug.LogError("Player not found for connection ID: " + playerState.v_playerConnectionID);
            }
        }

        Debug.Log("All players loaded on new client");
    }

    private IEnumerator<object> InitializeClientPlayer(PlayerSaveMessage message)
    {
        Player player = GetPlayerByConnectionID(message.connectionID);

        yield return new WaitUntil(() => player.isLoadedOrInitialized);

        player.InitializePlayer(message.playerSave, message.connectionID);
        player.connectionID = message.connectionID;
    }

    private Player GetPlayerByConnectionID(string connectionID)
    {
        foreach (GameObject playerGameobject in GameObject.FindGameObjectsWithTag("Player"))
        {
            Player player = playerGameobject.GetComponent<Player>();
            if (player.connectionID == connectionID)
            {
                return player;
            }
        }

        return null;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public override void OnStopHost()
    {
        base.OnStopHost();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadWorld(WorldSave worldSave)
    {
        GameManager.Instance.SetWorld(worldSave);
        GameManager.Instance.LoadGame();
    }

    public void StartGameAsHost(PlayerSave playerSave, WorldSave worldSave)
    {
        hostPlayerSave = playerSave;
        hostWorldSave = worldSave;

        StartHost();
    }

    public void FinalizeClientPlayer(PlayerSave playerSave)
    {
        clientPlayerSave = playerSave;

        // Ensure the world is loaded after selection
        LoadWorld(hostWorldSave);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        MainMenu.Instance.OnClientConnected();
    }
}
