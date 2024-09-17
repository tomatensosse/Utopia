
using System.Collections.Generic;
using UnityEngine;

public class RelativeBiomes
{
    public bool left = false;
    public bool right = false;
    public bool front = false;
    public bool back = false;

    public bool frontLeft = false;
    public bool frontRight = false;
    public bool backLeft = false;
    public bool backRight = false;

    public List<GameObject> all = new List<GameObject>();

    public void Feed(Vector2Int chunkPosition, MapGenerator generator)
    {
        int x = chunkPosition.x;
        int y = chunkPosition.y;

        int val = generator.map[x, y];

        if (generator.map[x + 1, y] != val)
        {
            left = true;
        }
        if (generator.map[x - 1, y] != val)
        {
            right = true;
        }
        if (generator.map[x, y + 1] != val)
        {
            back = true;
        }
        if (generator.map[x, y - 1] != val)
        {
            front = true;
        }

        if (generator.map[x + 1, y + 1] != val)
        {
            backLeft = true;
        }
        if (generator.map[x - 1, y  + 1] != val)
        {
            backRight = true;
        }
        if (generator.map[x + 1, y - 1] != val)
        {
            frontLeft = true;
        }
        if (generator.map[x - 1, y - 1] != val)
        {
            frontRight = true;
        }
    }

    public void DebugPrint(GameObject o, bool b)
    {
        if (b)
        {
            Debug.Log("Chunk " + o.name + " has " + all.Count + " relative chunks with different biomes around it.");

            for (int i = 0; i < all.Count; i++)
            {
                Debug.Log("Chunk " + o.name + " has biome " + all[i].GetComponent<Chunk>().chunkBiome + " around it.");
            }
        }
    }
}