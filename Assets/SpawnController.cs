using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
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

            for (int i = 0; i > World.Instance.maxSpawnsPerTick; i++)
            {
                float proceed = Random.Range(0f, 1f);

                if (proceed < 0.5f)
                {
                    break; /* (. ) ( .) */
                }

                int enemy = Random.Range(0, biome.NPCs.Count);
            }
        }
    }
}
