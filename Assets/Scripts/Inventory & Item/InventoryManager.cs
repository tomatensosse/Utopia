using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("UI")]
    public bool inventoryActive = false;
    public GameObject mainInventoryGroup;

    [Header("Inventory")]
    public Inventory inventory;
    public InventorySlot[] inventorySlots;
    public int inventorySize = 32; // 8 hotbar slots, 24 inventory slots
    public GameObject inventoryItemPrefab;
    private ItemDatabase itemDatabase;
    private Item oldItem;
    private int selectedSlot = -1;

    public void Initialize()
    {
        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();
        }

        if (itemDatabase == null)
        {
            itemDatabase = ItemDatabase.Instance;
        }

        GameObject grid = mainInventoryGroup = GameObject.Find("InventoryGrid");

        GameObject toolbar = GameObject.Find("Toolbar");

        int length = grid.transform.childCount + toolbar.transform.childCount;

        inventorySlots = new InventorySlot[length];

        for (int i = 0; i < toolbar.transform.childCount; i++)
        {
            inventorySlots[i] = toolbar.transform.GetChild(i).GetComponent<InventorySlot>();
        }

        for (int i = 0; i < grid.transform.childCount; i++)
        {
            inventorySlots[toolbar.transform.childCount + i] = grid.transform.GetChild(i).GetComponent<InventorySlot>();
        }

        inventory.Initialize(); // call on end of init

        ChangeSelectedSlot(0);

        GameUI.Instance.DisableTabs();
    }

    public void LoadItemsFromInventory()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            ItemData itemData = inventory.inventory[i];
            if (itemData != null)
            {
                Item item = itemDatabase.GetItemByID(itemData.itemID);
                SpawnNewItem(item, inventorySlots[i], itemData.itemAmount);
            }
        }
    }

    public void HotbarSwap(string inputString)
    {
        bool isNumber = int.TryParse(Input.inputString, out int number);
        if (isNumber && number > 0 && number < 9)
        {
            ChangeSelectedSlot(number - 1);
        }
    }

    void ChangeSelectedSlot(int newValue)
    {
        if (inventory.heldItem == null)
        {
            oldItem = null;
        }
        else
        {
            oldItem = itemDatabase.GetItemByID(inventory.heldItem.itemID);
        }

        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].Deselect();
        }
        inventorySlots[newValue].Select();
        selectedSlot = newValue;
        inventory.Hold(selectedSlot);
    }

    public bool AddItem(Item item, int amount = 1)
    {
        // Check if the slot has the same item with lower than max stack
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.item.isStackable && itemInSlot.count < item.itemMaxStack) {
                int remainingSpace = item.itemMaxStack - itemInSlot.count;
                if (amount <= remainingSpace)
                {
                    itemInSlot.count += amount;
                    itemInSlot.RefreshCount();
                    inventory.inventory[i].itemAmount += amount;
                    inventory.Push(true, selectedSlot);
                    return true;
                }
                else
                {
                    itemInSlot.count = item.itemMaxStack;
                    itemInSlot.RefreshCount();
                    inventory.inventory[i].itemAmount = item.itemMaxStack;
                    amount -= remainingSpace;
                }
            }
        }

        // Find any empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null) {
                SpawnNewItem(item, slot);
                inventory.inventory[i] = GenerateItemData(item, amount);
                inventory.Push(true, selectedSlot);
                return true;
            }
        }

        inventory.Push(true, selectedSlot);
        return false;
    }

    ItemData GenerateItemData(Item item, int amount = 1)
    {
        ItemData itemData = new ItemData();
        itemData.itemID = item.itemID;
        itemData.itemAmount = amount;
        itemData.itemDurability = item.itemMaxDurability; // generate durability
        return itemData;
    }

    void SpawnNewItem(Item item, InventorySlot slot, int amount = 1)
    {
        GameObject newItemGameObject = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGameObject.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item, this);
        inventoryItem.count = amount;
    }

    public void SwapItems(int index1, int index2)
    {
        Debug.Log("SwapItems called with index1: " + index1 + " and index2: " + index2);
        ItemData tempItem = inventory.inventory[index1];

        inventory.inventory[index1] = inventory.inventory[index2];

        inventory.inventory[index2] = tempItem;

        inventory.Push(true, selectedSlot);
    }

    public void MoveItem(int fromIndex, int toIndex)
    {
        Debug.Log("MoveItem called with fromIndex: " + fromIndex + " and toIndex: " + toIndex);
        inventory.inventory[toIndex] = inventory.inventory[fromIndex];

        inventory.inventory[fromIndex] = null;

        inventory.Push(true, selectedSlot);
    }
}
