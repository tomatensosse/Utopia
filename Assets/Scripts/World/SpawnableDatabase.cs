using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableDatabase : MonoBehaviour
{
    public static SpawnableDatabase Instance { get; private set; }
    public List<GameObject> spawnables;
    private Dictionary<string, GameObject> spawnableDictionary;

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

        spawnableDictionary = new Dictionary<string, GameObject>();

        foreach (GameObject spawnable in spawnables)
        {
            spawnableDictionary.Add(spawnable.GetComponent<Spawnable>().spawnableID, spawnable);
        }
    }

    public GameObject GetSpawnableByID(string spawnableID)
    {
        spawnableDictionary.TryGetValue(spawnableID, out GameObject spawnable);
        return spawnable;
    }

#if UNITY_EDITOR
    [ContextMenu("Populate Spawnable Database")]
    public void PopulateSpawnableDatabase()
    {
        spawnables.Clear();

        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:GameObject");
        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            // get each asset with spawnable component
            GameObject spawnable = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (spawnable.GetComponent<Spawnable>() != null)
            {
                spawnables.Add(spawnable);
            }
        }

        Debug.Log("Spawnable database populated with " + spawnables.Count + " spawnables.");
    }
#endif
}
