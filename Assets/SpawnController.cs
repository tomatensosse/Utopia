using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public static SpawnController Instance { get; private set; }

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

    private void OnEnable()
    {
        World.OnSpawnTick += SpawnEnemies;
    }

    private void OnDisable()
    {
        World.OnSpawnTick -= SpawnEnemies;
    }

    private void SpawnEnemies(object state)
    {
        foreach (PlayerLocation playerLocation in World.Instance.playerLocations)
        {
            Biome biome = MapGenerator.Instance.GetBiome(playerLocation.chunkPosition);

            for (int i = 0; i < World.Instance.maxSpawnsPerTick; i++)
            {
                float proceed = Random.Range(0f, 1f);

                if (proceed < 0.5f)
                {
                    break; /* (. ) ( .) */
                }

                Debug.Log("Spawning enemy");

                int enemy = Random.Range(0, biome.NPCs.Count);
                GameObject npc = biome.NPCs[enemy];
                GameObject npcGameObject = Instantiate(npc);

                Chunk chunk = GetSpawnChunk(playerLocation);

                Vector3 spawnPosition = chunk.GetRandomPosition();

                npcGameObject.transform.position = spawnPosition;

                NetworkServer.Spawn(npcGameObject);
            }
        }
    }

    private Chunk GetSpawnChunk(PlayerLocation playerLocation)
    {
        while (true)
        {
            int x = playerLocation.chunkPosition.x + Random.Range(-World.Instance.maximumSpawnDistance, World.Instance.maximumSpawnDistance);
            int z = playerLocation.chunkPosition.y + Random.Range(-World.Instance.maximumSpawnDistance, World.Instance.maximumSpawnDistance);

            if (x > World.Instance.minimumSpawnDistance | z > World.Instance.minimumSpawnDistance)
            {
                Chunk chunk = MeshGenerator.Instance.GetChunk(new Vector2Int(x, z));
                if (chunk != null)
                {
                    return chunk;
                }
            }
        }
    }
}
