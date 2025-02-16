using System.Collections.Generic;
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

    public void UpdateItem(int index, ItemInstance itemInstance)
    {
        if (index >= 0 && index < slots.Count)
        {
            if (slots[index].uiItem != null)
            {
                slots[index].uiItem.Initialize(itemInstance);
            }
            else
            {
                slots[index].SpawnUIItem(uiItemPrefab, itemInstance);
            }
        }
    }

    public void RemoveItem(int index)
    {
        if (index >= 0 && index < slots.Count)
        {
            slots[index].ClearSlot();
        }
    }

    public void InsertItem(int index, ItemInstance itemInstance)
    {
        if (index >= 0 && index < slots.Count)
        {
            // First shift items if needed
            for (int i = slots.Count - 1; i > index; i--)
            {
                if (slots[i-1].uiItem != null)
                {
                    if (slots[i].uiItem == null)
                    {
                        slots[i].SpawnUIItem(uiItemPrefab, slots[i-1].uiItem.itemInstance);
                    }
                    else
                    {
                        slots[i].uiItem.Initialize(slots[i-1].uiItem.itemInstance);
                    }
                }
            }
            
            // Then insert the new item
            if (slots[index].uiItem == null)
            {
                slots[index].SpawnUIItem(uiItemPrefab, itemInstance);
            }
            else
            {
                slots[index].uiItem.Initialize(itemInstance);
            }
        }
    }
}