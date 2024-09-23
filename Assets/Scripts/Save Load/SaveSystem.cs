using System.Collections.Generic;
using UnityEngine;

public class SaveSystem
{
    public void SavePlayer(PlayerData playerSave, string saveID)
    {
        playerSave.saveID = saveID;
        string json = JsonUtility.ToJson(playerSave);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/PlayerSaves/" + saveID + ".json", json);
    }

    public void SaveWorld(WorldData worldData, string saveID)
    {
        worldData.saveID = saveID;
        string json = JsonUtility.ToJson(worldData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/WorldSaves/" + saveID + ".json", json);
    }

    public PlayerData LoadPlayer(string saveID)
    {
        string json = System.IO.File.ReadAllText(Application.persistentDataPath + "/PlayerSaves/" + saveID + ".json");
        return JsonUtility.FromJson<PlayerData>(json);
    }

    public WorldData LoadWorld(string saveID)
    {   
        string json = System.IO.File.ReadAllText(Application.persistentDataPath + "/WorldSaves/" + saveID + ".json");
        return JsonUtility.FromJson<WorldData>(json);
    }

    public List<PlayerData> LoadAllPlayers()
    {
        List<PlayerData> playerSaves = new List<PlayerData>();
        if (!System.IO.Directory.Exists(Application.persistentDataPath + "/PlayerSaves/"))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/PlayerSaves/");
        }

        foreach (string path in System.IO.Directory.GetFiles(Application.persistentDataPath + "/PlayerSaves/"))
        {
            string json = System.IO.File.ReadAllText(path);
            playerSaves.Add(JsonUtility.FromJson<PlayerData>(json));
        }
        return playerSaves;
    }

    public List<WorldData> LoadAllWorlds()
    {
        List<WorldData> worldSaves = new List<WorldData>();
        if (!System.IO.Directory.Exists(Application.persistentDataPath + "/WorldSaves/"))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/WorldSaves/");
        }

        foreach (string path in System.IO.Directory.GetFiles(Application.persistentDataPath + "/WorldSaves/"))
        {
            string json = System.IO.File.ReadAllText(path);
            worldSaves.Add(JsonUtility.FromJson<WorldData>(json));
        }
        return worldSaves;
    }

    public static void DeleteSave(SaveData saveData)
    {
        if (saveData is PlayerData)
        {
            PlayerData playerSave = (PlayerData)saveData;
            System.IO.File.Delete(Application.persistentDataPath + "/PlayerSaves/" + playerSave.saveID + ".json");
        }
        else if (saveData is WorldData)
        {
            WorldData worldSave = (WorldData)saveData;
            System.IO.File.Delete(Application.persistentDataPath + "/WorldSaves/" + worldSave.saveID + ".json");
        }
    }
}
