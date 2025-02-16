using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI_Slot : MonoBehaviour
{
    public enum SlotType
    {
        Storage,
        Accessory,
        RightHand,
        LeftHand,
        Head,
        Chest,
        Legs,
        Feet,
        Mount,
        Pet,
    }

    public SlotType slotType;
    public int slotIndex = -1;
    public InventoryUI_Item uiItem;

    public void Initialize(int slotIndex)
    {
        this.slotIndex = slotIndex;
    }

    public void SpawnUIItem(GameObject uiItemPrefab, ItemInstance itemInstance)
    {
        GameObject uiItemGO = Instantiate(uiItemPrefab, transform);
        uiItem = uiItemGO.GetComponent<InventoryUI_Item>();
        uiItem.Initialize(slotIndex, itemInstance);
    }

    public void ClearSlot()
    {
        if (uiItem != null)
        {
            Destroy(uiItem.gameObject);
            uiItem = null;
        }
    }
}
