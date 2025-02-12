using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    public static SpawnPointManager Instance;
    public Transform[] playerSpawnPoints;
    public Transform[] enemySpawnPoints;
    public Transform[] lootSpawnPoints;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public Transform GetRandomPlayerSpawnPoint()
    {
        return playerSpawnPoints[Random.Range(0, playerSpawnPoints.Length)];
    }

    public Transform GetRandomEnemySpawnPoint()
    {
        return enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)];
    }

    public Transform GetRandomLootSpawnPoint()
    {
        return lootSpawnPoints[Random.Range(0, lootSpawnPoints.Length)];
    }
}