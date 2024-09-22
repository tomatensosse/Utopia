
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

    public void Feed(Vector2Int chunkPosition, int[,] biomeMap)
    {
        int x = chunkPosition.x;
        int y = chunkPosition.y;

        int val = biomeMap[x, y];

        if (biomeMap[x + 1, y] != val)
        {
            left = true;
        }
        if (biomeMap[x - 1, y] != val)
        {
            right = true;
        }
        if (biomeMap[x, y + 1] != val)
        {
            back = true;
        }
        if (biomeMap[x, y - 1] != val)
        {
            front = true;
        }

        if (biomeMap[x + 1, y + 1] != val)
        {
            backLeft = true;
        }
        if (biomeMap[x - 1, y  + 1] != val)
        {
            backRight = true;
        }
        if (biomeMap[x + 1, y - 1] != val)
        {
            frontLeft = true;
        }
        if (biomeMap[x - 1, y - 1] != val)
        {
            frontRight = true;
        }
    }
}