using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Spawnable
{
    [Range(0, 1)]
    public float generateChance = 0.3f;

    public bool hasVariations;

    [ConditionalHide(nameof(hasVariations), false)]
    public GameObject objectToSpawn;

    [ConditionalHide(nameof(hasVariations), true)]
    public List<GameObject> objectToSpawnVariations;

    public virtual GameObject GetGameObject()
    {
        if (!hasVariations)
        {
            return objectToSpawn;
        }
        else
        {
            int randomIndex = Random.Range(0, objectToSpawnVariations.Count);
            return objectToSpawnVariations[randomIndex];
        }
    }
    
    

}
