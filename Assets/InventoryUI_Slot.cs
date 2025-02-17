using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI_Slot : MonoBehaviour, IDropHandler
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

    #region Drag and Drop

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0) {
            InventoryUI_Item uiItem = eventData.pointerDrag.GetComponent<InventoryUI_Item>();
            uiItem.parentAfterDrag = transform;
        }
    }

    #endregion

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
