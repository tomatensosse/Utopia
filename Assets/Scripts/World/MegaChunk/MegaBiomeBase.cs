using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class MegaBiomeBase : ScriptableObject
{
    [System.Serializable]
    public struct BiomeGeneration
    {
        public Biome biome;
        [MinMaxSlider(0, 1)] public Vector2 threshold;
    }

    public BiomeGeneration defaultBiome;
    public List<BiomeGeneration> biomes = new List<BiomeGeneration>();

    public Biome GetBiomeAt(Vector3Int chunkPosition)
    {
        int seed = World.Seed;

        float noise = Mathf.PerlinNoise((seed + chunkPosition.x) * 0.1f, (seed + chunkPosition.z) * 0.1f);

        foreach (BiomeGeneration biome in biomes)
        {
            if (noise >= biome.threshold.x && noise <= biome.threshold.y)
            {
                return biome.biome;
            }
        }

        return defaultBiome.biome;
    }
}