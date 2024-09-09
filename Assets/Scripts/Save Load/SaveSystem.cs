using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string saveID;
}

[System.Serializable]
public class PlayerSave : SaveData
{
    public string playerSaveName;
    public string _demoPlayerClass;
    public int playerScore;
}

[System.Serializable]
public class WorldSave : SaveData
{
    public string worldSaveName;
    public int worldSeed;
    public List<EntityData> entityDatas;
}

public class SaveSystem
{
    public void SavePlayer(PlayerSave playerSave, string saveID)
    {
        playerSave.saveID = saveID;
        string json = JsonUtility.ToJson(playerSave);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/PlayerSaves/" + saveID + ".json", json);
    }

    public void SaveWorld(WorldSave worldSave, string saveID)
    {
        worldSave.saveID = saveID;
        string json = JsonUtility.ToJson(worldSave);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/WorldSaves/" + saveID + ".json", json);
    }

    public PlayerSave LoadPlayer(string saveID)
    {
        string json = System.IO.File.ReadAllText(Application.persistentDataPath + "/PlayerSaves/" + saveID + ".json");
        return JsonUtility.FromJson<PlayerSave>(json);
    }

    public WorldSave LoadWorld(string saveID)
    {   
        string json = System.IO.File.ReadAllText(Application.persistentDataPath + "/WorldSaves/" + saveID + ".json");
        return JsonUtility.FromJson<WorldSave>(json);
    }

    public List<PlayerSave> LoadAllPlayers()
    {
        List<PlayerSave> playerSaves = new List<PlayerSave>();
        if (!System.IO.Directory.Exists(Application.persistentDataPath + "/PlayerSaves/"))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/PlayerSaves/");
        }

        foreach (string path in System.IO.Directory.GetFiles(Application.persistentDataPath + "/PlayerSaves/"))
        {
            string json = System.IO.File.ReadAllText(path);
            playerSaves.Add(JsonUtility.FromJson<PlayerSave>(json));
        }
        return playerSaves;
    }

    public List<WorldSave> LoadAllWorlds()
    {
        List<WorldSave> worldSaves = new List<WorldSave>();
        if (!System.IO.Directory.Exists(Application.persistentDataPath + "/WorldSaves/"))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/WorldSaves/");
        }

        foreach (string path in System.IO.Directory.GetFiles(Application.persistentDataPath + "/WorldSaves/"))
        {
            string json = System.IO.File.ReadAllText(path);
            worldSaves.Add(JsonUtility.FromJson<WorldSave>(json));
        }
        return worldSaves;
    }
}
