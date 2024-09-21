using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData
{
    public List<Vector3> vertices;
    public int[] triangles;

    public List<SpawnableData> spawnableDatas;
}
