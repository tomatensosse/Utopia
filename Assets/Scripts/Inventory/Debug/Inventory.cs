/// <summary>
/// 
/// TODO:
/// - Fix custom JSON serialization for ItemData
/// 
/// </summary>

using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
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
    public void Add() 
    {
        int randomAmount = Random.Range(1, 20);
        int randomDurability = Random.Range(1, 10);

        AddItem(RandomDemoItem(), randomAmount, randomDurability); 
    }
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

    private void Awake()
    {
        InitializeInventory();
    }

    private void InitializeInventory()
    {
        inventory_itemDatas = new ItemData[inventorySize];
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

    private Item RandomDemoItem()
    {
        int randomIndex = Random.Range(-1, itemsForDemo.Count);

        if (randomIndex == -1)
        {
            return null;
        }

        Item item = itemsForDemo[randomIndex];

        Debug.Log($"Random item: {item.ItemName}");

        return item;
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

    private int NextEmptySlot()
    {
        for (int i = 0; i < inventory_itemDatas.Length; i++)
        {
            Debug.Log(string.IsNullOrEmpty(inventory_itemDatas[i].itemID));

            if (string.IsNullOrEmpty(inventory_itemDatas[i].itemID))
            {
                return i;
            }
        }

        Debug.Log("No empty slots.");
        return -1;
    }

    private int NextItemSlot(string itemID, bool checkForStackable = true)
    {
        for (int i = 0; i < inventory_itemDatas.Length; i++)
        {
            if (inventory_itemDatas[i] != null && inventory_itemDatas[i].itemID == itemID)
            {
                if (checkForStackable && ItemDatabase.Instance.GetItemByID(itemID).IsStackable && inventory_itemDatas[i].itemAmount < ItemDatabase.Instance.GetItemByID(itemID).MaxStackSize)
                {
                    return i;
                }

                if (!checkForStackable)
                {
                    return i;
                }
            }
        }

        Debug.Log($"No slots with {itemID}.");
        return -1;
    }

    // TBA
    private bool AddItem(Item item, int amount = 1, int durability = -1)
    {
        if (item == null)
        {
            return false; // change logic to your preference | should an empty item return true or false?
        }

        ItemData itemSerialized = new ItemData().Generate(item.ItemID, amount, durability);

        // Check if the item is stackable
        if (item.IsStackable)
        {
            int slot;

            if (inventory_itemDatas == null)
            {
                Debug.LogError("Inventory is null.");
            }

            // Single item if item is not stackable
            if (!item.IsStackable)
            {
                slot = NextEmptySlot();

                if (slot == -1)
                {
                    return false;
                }

                inventory_itemDatas[slot] = itemSerialized;

                AddToMaterialsDictionary(item.ItemID, amount); // Log total amount of material

                return true;
            }

            slot = NextItemSlot(item.ItemID);

            bool slotIsEmpty = false;

            if (slot == -1)
            {
                slot = NextEmptySlot();
                slotIsEmpty = true;
            }

            int stackSize = item.MaxStackSize;

            // Stack the item if possible on pre-occupied slot
            if (slot != -1 && !slotIsEmpty)
            {
                if (inventory_itemDatas[slot].itemAmount + amount <= stackSize)
                {
                    inventory_itemDatas[slot].itemAmount += amount;

                    AddToMaterialsDictionary(item.ItemID, amount); // Log total amount of material

                    return true;
                }
                else
                {
                    int remaining = stackSize - inventory_itemDatas[slot].itemAmount;
                    inventory_itemDatas[slot].itemAmount = stackSize;

                    AddToMaterialsDictionary(item.ItemID, remaining); // Log total amount of material

                    return AddItem(item, amount - remaining);
                }
            }

            // Stack the item if possible on empty slot
            if (slot != -1 && slotIsEmpty)
            {
                inventory_itemDatas[slot] = itemSerialized;

                if (amount > stackSize)
                {
                    inventory_itemDatas[slot].itemAmount = stackSize;

                    AddToMaterialsDictionary(item.ItemID, stackSize); // Log total amount of material

                    int remaining = amount - stackSize;
                    return AddItem(item, remaining);
                }

                return true;
            }
        }

        return false;
    }
}