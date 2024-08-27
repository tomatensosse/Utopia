/// <summary>
/// 
/// TODO:
/// - Fix custom JSON serialization for ItemData
/// 
/// </summary>

using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class DebugItemSpawn : MonoBehaviour
{
    [Header("Display")]
    public Item heldItem;
    public ItemData heldItemData;
    public int selectedSlot = 0;
    [Header("References")]
    public string savePath = "Assets/Scripts/Inventory/Debug/InventorySave.json";
    public List<Item> itemsForDemo;
    public int inventorySize = 6;
    public ItemData[] inventory_itemDatas;
    public void Save() { ItemsToJson(); } // Save the inventory to a JSON file
    public void Load() { inventory_itemDatas = ItemsFromJson(); } // Load the inventory from a JSON file

    private void Start()
    {
        GenerateDemoInventory();
    }

    private void Update()
    {
        if (Input.inputString != null)
        {
            ChangeSelectedSlot(Input.inputString);
        }
    }

    private void ChangeSelectedSlot(string input)
    {
        if (int.TryParse(input, out int result))
        {
            selectedSlot = result - 1;
        }

        if (inventory_itemDatas[selectedSlot] != null)
        {
            heldItem = ItemDatabase.Instance.GetItemByID(inventory_itemDatas[selectedSlot].itemID);
            heldItemData = inventory_itemDatas[selectedSlot];
        }
        else 
        {
            heldItem = null;
            heldItemData = null;
        }
    }

    public void ItemsToJson()
    {
        string items = "{\"items\": [";

        foreach (ItemData itemData in inventory_itemDatas)
        {
            if (itemData != null)
            {
                items += itemData.ItemToJson();
            }

            if (itemData != inventory_itemDatas[inventory_itemDatas.Length - 1])
            {
                items += ",";
            }
        }

        items += "]}";

        System.IO.File.WriteAllText(savePath, items);
    }

    public ItemData[] ItemsFromJson()
    {
        ItemData[] itemDatas = new ItemData[inventorySize];

        string json = System.IO.File.ReadAllText(savePath);

        // Parse the JSON string
        var jsonObject = JsonUtility.FromJson<InventoryJson>(json);

        // Convert each JSON item back into an ItemData object
        for (int i = 0; i < jsonObject.items.Length; i++)
        {
            itemDatas[i] = jsonObject.items[i];
        }

        return itemDatas;
    }

    [System.Serializable]
    private class InventoryJson
    {
        public ItemData[] items;
    }

    public void GenerateDemoInventory()
    {
        inventory_itemDatas = new ItemData[inventorySize];

        for (int i = 0; i < inventory_itemDatas.Length; i++)
        {
            int randomIndex = Random.Range(-1, itemsForDemo.Count);

            if (randomIndex == -1)
            {
                inventory_itemDatas[i] = null;
            }
            else
            {
                Item item = itemsForDemo[randomIndex];
                inventory_itemDatas[i] = new ItemData().Generate(item.ItemID, Random.Range(1, 10), Random.Range(1, 100));
            }
        }
    }

    // TBA
    private bool AddItemToInventory(Item item, int amount = 1, int durability = -1)
    {
        ItemData itemSerialized = new ItemData().Generate(item.ItemID, amount, durability);

        return false;
    }
}
