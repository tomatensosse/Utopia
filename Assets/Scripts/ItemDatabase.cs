using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }
    public Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        SetupDatabase();
    }

    private void SetupDatabase()
    {
        Item[] items = Resources.LoadAll<Item>("Items");

        foreach (Item item in items)
        {
            itemDictionary.Add(item.uid, item);
        }

        Debug.Log($"{itemDictionary.Count} items loaded into database.");
    }

    public Item GetItem(string uid)
    {
        if (itemDictionary.ContainsKey(uid))
        {
            return itemDictionary[uid];
        }

        Debug.LogError("Item with UID " + uid + " not found in database.");
        return null;
    }
}
