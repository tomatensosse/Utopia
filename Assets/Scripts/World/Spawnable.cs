using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Spawnable : MonoBehaviour
{
    public string spawnableID;
    [Range(0, 1)]
    public float generateChance = 0.3f;

    public bool hasVariations;

    public virtual SpawnableData GetSpawnableData()
    {
        SpawnableData spawnableData = new SpawnableData();
        
        spawnableData.spawnableID = spawnableID;
        spawnableData.localPosition = transform.localPosition;
        spawnableData.localRotation = transform.localRotation.eulerAngles;
        spawnableData.localScale = transform.localScale;

        return spawnableData;
    }
}
