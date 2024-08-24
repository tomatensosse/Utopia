using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }
    public List<Entity> entities = new List<Entity>();
    private SaveSystem saveSystem = new SaveSystem();
    private PlayerSave playerSave;
    private WorldSave worldSave;

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
    }

    public void Start()
    {
        LoadGame(GameManager.Instance.setPlayer, GameManager.Instance.setWorld);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SaveAndQuit();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            // demo
            GameObject demoEnemy = Instantiate(EntityDatabase.Instance.GetEntityByID("entity_physicube"));
            NetworkServer.Spawn(demoEnemy);
        }
    }

    public void LoadGame(PlayerSave loadPlayerSave, WorldSave loadWorldSave)
    {
        playerSave = loadPlayerSave;
        worldSave = loadWorldSave;
        Debug.Log("Loaded game with player: " + playerSave.playerSaveName + " and world: " + worldSave.worldSaveName);

        foreach (EntityData entityData in worldSave.entityDatas)
        {
            GameObject entityPrefab = EntityDatabase.Instance.GetEntityByID(entityData.entityID);
            GameObject entity = Instantiate(entityPrefab);
            entity.GetComponent<Entity>().LoadEntity(entityData);
        }
        // use player and world data to properly load the game
    }

    public void SaveGame()
    {
        if (worldSave.entityDatas == null)
        {
            worldSave.entityDatas = new List<EntityData>();
        }

        foreach (Entity entity in entities)
        {
            worldSave.entityDatas.Add(entity.SaveEntity());
        }

        saveSystem.SaveWorld(worldSave, worldSave.saveID);
    }

    public void SaveAndQuit(bool closeGame = false)
    {
        Debug.Log("Saved game");
        SaveGame();

        if (closeGame)
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
