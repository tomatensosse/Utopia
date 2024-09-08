using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // private fields
    [SerializeField] public ItemData[] inventory; // Public for debug purposes
    private int inventorySize = 32; // 8 hotbar slots + 24 inventory slots

    // public fields
    public string savePath = "Assets/Scripts/Inventory/inventory.json";
    public List<Item> demoItems; // Demo items to add to the inventory

    void Awake()
    {
        inventory = new ItemData[inventorySize];
    }

    public void LoadInventory()
    {
        InventoryData data = InventoryData.LoadFromFile(savePath);
        inventory = data.inventory;

        for (int i = 0; i < inventory.Length; i++)
        {
            InventoryManager.Instance.LoadItem(inventory[i], i);
        }

        Debug.Log("Inventory loaded from file.");
    }

    public void SaveInventory()
    {
        InventoryData data = new InventoryData();
        data.inventory = inventory;
        data.SaveToFile(savePath);

        Debug.Log("Inventory saved to file.");
    }

    public void DemoAddItem()
    {
        int index = Random.Range(0, demoItems.Count);
        Item item = demoItems[index];

        int amount = Random.Range(1, item.MaxStack);
        ItemData itemData = ItemData.Generate(item.ItemID, amount);

        Add(item, amount, item.MaxDurability);
    }

    public bool Add(Item item, int amount, int durability)
    {
        // Check for the next empty slot
        int slot = FindSlot(item.ItemID);

        if (slot == -1)
        {
            Debug.Log("Inventory is full."); // Return false if inventory full

            return false;
        }

        if (string.IsNullOrEmpty(inventory[slot].itemID)) // Empty slot
        {
            ItemData itemData = ItemData.Generate(item.ItemID, amount, durability);

            inventory[slot] = itemData;
            InventoryManager.Instance.LoadItem(itemData, slot);

            return true;
        }

        if (inventory[slot]._itemAmount + amount > item.MaxStack)
        {
            int remaining = item.MaxStack - inventory[slot]._itemAmount;
            inventory[slot]._itemAmount = item.MaxStack;
            amount -= remaining;
            InventoryManager.Instance.Refresh(slot, inventory[slot]._itemAmount, inventory[slot]._itemDurability);

            if (amount > 0)
            {
                Add(item, amount, durability);
            }

            return true;
        }
        else
        {
            inventory[slot]._itemAmount += amount;
            amount = 0;
            InventoryManager.Instance.Refresh(slot, inventory[slot]._itemAmount, inventory[slot]._itemDurability);

            return true;
        }
    }

    public void DebugPrintItems()
    {
        foreach (ItemData item in inventory)
        {
            Debug.Log($"Item: ID={item.itemID}, Amount={item.itemAmount}, Durability={item.itemDurability}");
        }
    }

    public int FindSlot(string itemID)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].itemID == itemID) // Found slot with same item
            {
                if (inventory[i].itemAmount < ItemDatabase.GetItemByID(itemID).MaxStack)
                {
                    return i;
                }
            }
        }

        for (int i = 0; i < inventory.Length; i++)
        {
            if (string.IsNullOrEmpty(inventory[i].itemID))
            {
                return i;
            }
        }

        return -1;
    }
}
