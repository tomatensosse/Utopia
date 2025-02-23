using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class InventoryUI : UISubclass
{
    public static InventoryUI Instance { get; private set; }
    public GameObject uiItemPrefab;
    public List<InventoryUI_Slot> slots;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public override void EnableUI()
    {
        base.EnableUI();

        Cursor.lockState = CursorLockMode.Confined;
    }

    public override void DisableUI()
    {
        base.DisableUI();

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Initialize(List<ItemInstance> itemInstances = null)
    {
        if (itemInstances == null)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].Initialize(i);
            }

            return;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < itemInstances.Count)
            {
                slots[i].SpawnUIItem(uiItemPrefab, itemInstances[i]);
            }
            else
            {
                slots[i].Initialize(i);
            }
        }
    }

    public void AddNewItem(ItemInstance itemInstance)
    {   
        itemInstance.Deserialize();

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].uiItem == null)
            {
                slots[i].SpawnUIItem(uiItemPrefab, itemInstance);
                return;
            }
        }
    }

    public void UpdateItem(ItemInstance itemInstance)
    {
        itemInstance.Deserialize();

        InventoryUI_Slot slot = GetSlotWithItem(itemInstance);

        if (slot != null)
        {
            slot.uiItem.UpdateAmount(itemInstance.amount);

            Debug.Log($"Updated item amount to {itemInstance.amount}.");

            return;
        }

        Debug.LogError("Failed to update item amount.");
    }

    private InventoryUI_Slot GetSlotWithItem(ItemInstance itemInstance)
    {
        foreach (InventoryUI_Slot slot in slots)
        {
            if (slot.uiItem != null)
            {
                if (slot.uiItem.itemInstance.instanceUID == itemInstance.instanceUID)
                {
                    return slot;
                }
            }
        }

        Debug.LogWarning("No slot found with itemInstance.");

        return null;
    }
}