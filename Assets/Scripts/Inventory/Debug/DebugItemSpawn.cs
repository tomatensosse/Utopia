/// <summary>
/// 
/// TODO:
/// - Fix custom JSON serialization for ItemData
/// 
/// </summary>

using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class DebugItemSpawn : MonoBehaviour
{
    [Header("Display")]
    public Item heldItem;
    public ItemData heldItemData;
    public int selectedSlot = 0;
    [Header("Crafting")]
    public Blueprint selectedBlueprint;
    public Dictionary<string, int> materials = new Dictionary<string, int>(); // Not displayed in the inspector because unity
    [Header("References")]
    public string savePath = "Assets/Scripts/Inventory/Debug/InventorySave.json";
    public List<Item> itemsForDemo;
    public int inventorySize = 6;
    public ItemData[] inventory_itemDatas;
    public void Save() { ItemsToJson(); } // Save the inventory to a JSON file
    public void Load() { inventory_itemDatas = ItemsFromJson(); } // Load the inventory from a JSON file
    public void Craft()
    {
        if (TryCraftItem())
        {
            Debug.Log($"Crafted {selectedBlueprint.itemToCraft}.");
        }
        else
        {
            Debug.LogWarning($"Failed to craft {selectedBlueprint.itemToCraft}.");
        }
    }

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

    private void ItemsToJson()
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

    private ItemData[] ItemsFromJson()
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

    private void GenerateDemoInventory()
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
                int randomAmount = Random.Range(1, 20);
                int randomDurability = Random.Range(1, 10);
                inventory_itemDatas[i] = new ItemData().Generate(item.ItemID, randomAmount, randomDurability);

                AddToMaterialsDictionary(item.ItemID, randomAmount);
            }
        }

        PrintMaterialCounts();
    }

    private void AddToMaterialsDictionary(string itemID, int amount)
    {
        if (materials.ContainsKey(itemID))
        {
            materials[itemID] += amount;
        }
        else
        {
            materials.Add(itemID, amount);
        }
    }

    private bool TryCraftItem()
    {
        if (selectedBlueprint == null)
        {
            Debug.LogWarning("No blueprint selected.");
            return false;
        }

        foreach (Blueprint.BlueprintMaterial material in selectedBlueprint.materials)
        {
            if (!materials.ContainsKey(material.materialID) || materials[material.materialID] < material.materialAmount)
            {
                Debug.LogWarning($"Not enough {material.materialID} to craft {selectedBlueprint.itemToCraft}.");
                return false;
            }
        }

        foreach (Blueprint.BlueprintMaterial material in selectedBlueprint.materials)
        {
            materials[material.materialID] -= material.materialAmount;
        }

        PrintMaterialCounts();

        return true; //AddItemToInventory(ItemDatabase.Instance.GetItemByID(selectedBlueprint.itemToCraft));
    }

    private void PrintMaterialCounts()
    {
        foreach (KeyValuePair<string, int> material in materials)
        {
            Debug.Log($"Material: {material.Key}, Amount: {material.Value}");
        }
    }

    // TBA
    private bool AddItemToInventory(Item item, int amount = 1, int durability = -1)
    {
        ItemData itemSerialized = new ItemData().Generate(item.ItemID, amount, durability);

        // Add to materials dictionary

        // Count empty space in inventory for better performance than checking every slot

        return false;
    }
}
