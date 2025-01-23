using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public WorldData newWorldData;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [Button(Name = "New World")]
    public void NewWorld()
    {
        if (newWorldData == null)
        {
            Debug.LogError("No world data found");
            return;
        }

        SaveSystem.SaveData(newWorldData);
    }

    [Button(Name = "Load Worlds")]
    public void LoadWorlds()
    {
        List<Data> datas = SaveSystem.LoadAll<WorldData>();

        Debug.Log("Loaded " + datas.Count + " worlds");
    }
}