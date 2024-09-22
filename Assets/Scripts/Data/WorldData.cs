using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData : SaveData
{
    public string worldSaveName;
    public int worldSeed;
    public List<EntityData> entityDatas;
    public List<ChunkData> chunkDatas;
}
