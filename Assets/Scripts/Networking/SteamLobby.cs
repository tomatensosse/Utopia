using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamLobby : MonoBehaviour
{
    public CSteamID lobbyID;
    private const int maxPlayers = 4;
    private Callback<GameLobbyJoinRequested_t> lobbyJoinRequested;
    private Callback<LobbyEnter_t> lobbyEntered;

    private void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steamworks not initialized");
            return;
        }
    }

    public void CreateLobby()
    {
        SteamAPICall_t handle = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, maxPlayers);
        SteamMatchmaking.SetLobbyJoinable(lobbyID, true);

        Debug.Log(SteamUser.GetSteamID().ToString());

        Callback<LobbyCreated_t>.Create(OnLobbyCreated);
    }

    public void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult == EResult.k_EResultOK)
        {
            lobbyID = new CSteamID(result.m_ulSteamIDLobby);
            
            Debug.Log("Lobby created: " + lobbyID);

            SteamMatchmaking.SetLobbyData(lobbyID, "HostAddress", SteamUser.GetSteamID().ToString());
            Debug.Log(lobbyID + " | " + SteamUser.GetSteamID().ToString());
        }
        else
        {
            Debug.LogError("Failed to create lobby");
        }
    }

    public void InviteFriend(CSteamID friendID)
    {
        SteamMatchmaking.InviteUserToLobby(lobbyID, friendID);
    }

    private void OnEnable()
    {
        lobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
    }

    private void OnDisable()
    {
        //lobbyJoinRequested.Dispose();
        //lobbyEntered.Dispose();
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t result)
    {
        SteamMatchmaking.RequestLobbyData(result.m_steamIDLobby);
        SteamMatchmaking.JoinLobby(result.m_steamIDLobby);
    }

    private void OnLobbyEnter(LobbyEnter_t result)
    {
        if (result.m_EChatRoomEnterResponse == (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
        {
            lobbyID = new CSteamID(result.m_ulSteamIDLobby);
            Debug.Log("Lobby entered: " + lobbyID);

            CustomNetworkManager networkManager = FindObjectOfType<CustomNetworkManager>();

            Debug.Log("Joining lobby: " + lobbyID);
            Debug.Log("Host address: " + SteamMatchmaking.GetLobbyData(lobbyID, "HostAddress"));

            networkManager.networkAddress = SteamMatchmaking.GetLobbyData(lobbyID, "HostAddress");
            networkManager.StartClient();
        }
    }
}
