using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private SaveSystem saveSystem = new SaveSystem();

    [Header("Save Data")]
    public PlayerSave setPlayer;
    public WorldSave setWorld;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayer(PlayerSave playerSave)
    {
        Debug.Log("Setting player: " + playerSave.playerSaveName);

        setPlayer = playerSave;
    }

    public void SetWorld(WorldSave worldSave)
    {
        Debug.Log("Setting world: " + worldSave.worldSaveName);

        setWorld = worldSave;
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }
}
