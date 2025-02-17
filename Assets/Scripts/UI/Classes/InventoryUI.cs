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

    public void UpdateItem(int index, int oldAmount, int newAmount)
    {
        slots[index].uiItem.UpdateAmount(oldAmount, newAmount);
    }
}