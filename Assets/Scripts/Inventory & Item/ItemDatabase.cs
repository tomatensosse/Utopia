using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }
    public List<Item> items;
    private Dictionary<string, Item> itemDictionary;

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
            itemDictionary.Add(item.itemID, item);
        }
    }

    public Item GetItemByID(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
        {
            return null;
        }
        itemDictionary.TryGetValue(itemID, out Item item);
        return item;
    }

#if UNITY_EDITOR
    [ContextMenu("Populate Item Database")]
    public void PopulateItemDatabase()
    {
        items.Clear();

        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Item");
        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            // get each asset with item component
            Item item = UnityEditor.AssetDatabase.LoadAssetAtPath<Item>(path);
            items.Add(item);
        }

        Debug.Log("Item database populated with " + items.Count + " items.");
    }
#endif
}
