using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }
    public List<Item> items;
    private Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();

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

        itemDictionary = new Dictionary<string, Item>();

        foreach (Item item in items)
        {
            itemDictionary.Add(item.ItemID, item);
        }
    }

    public Item GetItemByID(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
        {
            return null;
        }

        if (itemDictionary.ContainsKey(itemID))
        {
            return itemDictionary[itemID];
        }

        Debug.LogWarning($"Item with ID {itemID} not found in the database.");

        return null;
    }

#if UNITY_EDITOR
    [ContextMenu("Populate Item Database")]
    public void PopulateItemDatabase()
    {
        // Clear the current list to avoid duplicates
        items.Clear();

        // Find all Item ScriptableObjects in the project
        string[] guids = AssetDatabase.FindAssets("t:Item");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Item item = AssetDatabase.LoadAssetAtPath<Item>(path);
            if (item != null)
            {
                items.Add(item);
            }
        }

        itemDictionary = new Dictionary<string, Item>();

        foreach (Item item in items)
        {
            itemDictionary.Add(item.ItemID, item);
        }

        // Optionally, log the results for debugging
        Debug.Log($"Found and added {items.Count} items to the database.");
    }
#endif
}
