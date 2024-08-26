using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    public static SpawnPointManager Instance { get; private set; }

    public List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> itemSpawnPoints = new List<Transform>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There are multiple SpawnPointManager instances in the scene.");
            Destroy(gameObject);
        }
    }

    public Transform GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

    public Transform GetItemSpawnPoint()
    {
        int randomIndex = Random.Range(0, itemSpawnPoints.Count);
        Transform spawnPoint = itemSpawnPoints[randomIndex];
        itemSpawnPoints.RemoveAt(randomIndex);
        return spawnPoint;
    }
}
