using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDatabase : MonoBehaviour
{
    public static EntityDatabase Instance { get; private set; }
    public List<GameObject> entities;
    private Dictionary<string, GameObject> entityDictionary;

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

        entityDictionary = new Dictionary<string, GameObject>();

        foreach (GameObject entity in entities)
        {
            entityDictionary.Add(entity.GetComponent<Entity>().entityID, entity);
        }
    }

    public GameObject GetEntityByID(string entityID)
    {
        entityDictionary.TryGetValue(entityID, out GameObject entity);
        return entity;
    }

    public string RandomEntityID()
    {
        return entities[Random.Range(0, entities.Count)].GetComponent<Entity>().entityID;
    }

#if UNITY_EDITOR
    [ContextMenu("Populate Entity Database")]
    public void PopulateEntityDatabase()
    {
        entities.Clear();

        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:GameObject");
        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            // get each asset with enitity component
            GameObject entity = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (entity.GetComponent<Entity>() != null)
            {
                entities.Add(entity);
            }
        }

        Debug.Log("Entity database populated with " + entities.Count + " entities.");
    }
#endif
}
