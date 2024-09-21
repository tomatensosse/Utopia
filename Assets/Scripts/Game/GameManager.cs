using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private SaveSystem saveSystem = new SaveSystem();

    [Header("Save Data")]
    public PlayerData setPlayer;
    
    public WorldData setWorld;

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

    public void SetPlayer(PlayerData playerSave)
    {
        Debug.Log("Setting player: " + playerSave.playerSaveName);
        setPlayer = playerSave;
    }

    public void SetWorld(WorldData worldSave)
    {
        Debug.Log("Setting world: " + worldSave.worldSaveName);
        setWorld = worldSave;
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }
}
