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
    private InventoryManager inventoryManager;
    private static Inventory LocalInstance;
    private bool inventoryFullyLoaded = false;

    [Header("Display")]
    public Item heldItem;
    public ItemData heldItemData;
    public int selectedSlot = 0;
    [Header("Crafting")]
    public Blueprint selectedBlueprint;
    public Dictionary<string, int> materials = new Dictionary<string, int>(); // Not displayed in the inspector because unity
    [Header("References")]
    public string savePath = "Assets/Scripts/Inventory/Debug/";
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
    public void TryUseItem()
    {
        if (heldItem != null)
        {
            Debug.Log($"Used {heldItem.ItemName}.");

            heldItemData.durability--;
            inventoryManager.ChangeItemDurability(selectedSlot, heldItemData.durability);
        }
        else
        {
            Debug.LogWarning("No item held.");
        }
    }

    private void Awake()
    {
        // if (isLocalPlayer) { LocalInstance = this; }
    }

    private void Start()
    {
        inventoryManager = InventoryManager.Instance;

        inventoryManager.Initialize(inventorySize);

        inventory_itemDatas = new ItemData[inventorySize];
        inventoryFullyLoaded = true;
        //InitializeInventory();
    }

    private void InitializeInventory()
    {
        if (System.IO.File.Exists(savePath + "InventorySave.json"))
        {
            // if load exists
            Load();
        }
        else
        {
            // if load does not exist
            inventory_itemDatas = new ItemData[inventorySize];
            inventoryFullyLoaded = true;
        }
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

        System.IO.File.WriteAllText(savePath + "InventorySave.json", items);
    }

    private ItemData[] ItemsFromJson()
    {
        ItemData[] itemDatas = new ItemData[inventorySize];

        string json = System.IO.File.ReadAllText(savePath + "InventorySave.json");

        // Parse the JSON string
        var jsonObject = JsonUtility.FromJson<InventoryJson>(json);

        // Convert each JSON item back into an ItemData object
        for (int i = 0; i < jsonObject.items.Length; i++)
        {
            ItemData itemData = new ItemData();
            itemDatas[i] = itemData.JsonToItem(jsonObject.items[i].ItemToJson());
            Debug.Log(itemData.itemAmount);
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

        PrintMaterialCounts();
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
            if (!materials.ContainsKey(material.materialItem.ItemID) || materials[material.materialItem.ItemID] < material.materialItemAmount)
            {
                Debug.LogWarning($"Not enough {material.materialItem.ItemID} to craft {selectedBlueprint.itemToCraft}.");
                return false;
            }
        }

        foreach (Blueprint.BlueprintMaterial material in selectedBlueprint.materials)
        {
            materials[material.materialItem.ItemID] -= material.materialItemAmount;

            int remaining = material.materialItemAmount;

            while (remaining > 0)
            {
                int slot = NextItemSlot(material.materialItem.ItemID, false);

                if (slot == -1)
                {
                    Debug.LogError($"Not enough {material.materialItem.ItemID} to craft {selectedBlueprint.itemToCraft}. | Should'nt have reached this scope.");
                    return false;
                }

                if (inventory_itemDatas[slot].itemAmount > remaining)
                {
                    inventory_itemDatas[slot].itemAmount -= remaining;
                    remaining = 0;
                }
                else
                {
                    remaining -= inventory_itemDatas[slot].itemAmount;
                    inventory_itemDatas[slot] = null;
                }
            }
        }

        Item itemToCraft = selectedBlueprint.itemToCraft;
        AddItem(itemToCraft, selectedBlueprint.craftYield, itemToCraft.MaxDurability);

        PrintMaterialCounts();

        return true; //AddItemToInventory(ItemDatabase.Instance.GetItemByID(selectedBlueprint.itemToCraft));
    }

    private void PrintMaterialCounts()
    {
        string materialsString = "Materials:\n";

        foreach (KeyValuePair<string, int> material in materials)
        {
            materialsString += $"Material: {material.Key}, Amount: {material.Value}\n";
        }

        Debug.Log(materialsString);
    }

    private int NextEmptySlot()
    {   
        for (int i = 0; i < inventory_itemDatas.Length; i++)
        {
            if (inventory_itemDatas[i] == null)
            {
                return i;
            }

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
                if (checkForStackable && ItemDatabase.Instance.GetItemByID(itemID).MaxStackSize > 1 && inventory_itemDatas[i].itemAmount < ItemDatabase.Instance.GetItemByID(itemID).MaxStackSize)
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

        int slot;

        if (inventory_itemDatas == null)
        {
            Debug.LogError("Inventory is null.");
        }

        // Single item if item is not stackable
        if (item.MaxStackSize == -1)
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

        if (item.MaxStackSize == 0 || item.MaxStackSize == 1)
        {
            Debug.LogError("Item stack size is invalid."); // -1 for non stackable. Minimum of 2 for stackable
            return false;
        }

        // Check if the item is stackable
        if (item.MaxStackSize > 1)
        {
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

                AddToMaterialsDictionary(item.ItemID, amount); // Log total amount of material

                return true;
            }
        }

        return false;
    }
}
