using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public InventorySlotUI[] inventorySlotUIs; // Array of inventory slot UIs ; 32 slots | 8 hotbar 24 inventory
    public GameObject inventoryItemUIPrefab;
    public Inventory inventory;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadItem(ItemData itemData, int atIndex)
    {
        if (string.IsNullOrEmpty(itemData.itemID))
        {
            return;
        }

        if (inventorySlotUIs[atIndex].transform.childCount != 0)
        {
            Debug.LogError("Trying to load item into a non-empty slot");
            return;
        }

        GameObject itemObj = Instantiate(inventoryItemUIPrefab, inventorySlotUIs[atIndex].transform);
        InventoryItemUI itemUI = itemObj.GetComponent<InventoryItemUI>();
        itemUI.InitializeItem(itemData);
    }

    public void Refresh(int index, int amount, int durability)
    {
        InventoryItemUI itemUI = inventorySlotUIs[index].GetComponentInChildren<InventoryItemUI>();
        if (itemUI != null)
        {
            itemUI.RefreshAmount(amount);
            itemUI.RefreshDurability(durability);
        }
    }

    // Move and Swap items

    public void MoveItem(int index1, int index2)
    {
        ItemData temp = inventory.inventory[index1];
        inventory.inventory[index1] = inventory.inventory[index2];
        inventory.inventory[index2] = temp;
    }

    public void SwapItems(int index1, int index2)
    {
        ItemData temp = inventory.inventory[index1];
        inventory.inventory[index1] = inventory.inventory[index2];
        inventory.inventory[index2] = temp;
    }

    public int ReturnIndexOf(InventorySlotUI slot)
    {
        for (int i = 0; i < inventorySlotUIs.Length; i++)
        {
            if (inventorySlotUIs[i] == slot)
            {
                return i;
            }
        }

        return -1;
    }
}
