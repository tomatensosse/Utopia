using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI_Slot : MonoBehaviour, IDropHandler
{
    public SlotType slotType;
    public int slotIndex = -1;
    public InventoryUI_Item uiItem; // When UI Item Changes, Check for abilities...

    #region Drag and Drop

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0) // Later, add swapping with childCount != 0...
        {
            InventoryUI_Item uiItem = eventData.pointerDrag.GetComponent<InventoryUI_Item>();
            if (uiItem.itemInstance.itemReference.isEquippable)
            {
                if (slotType != uiItem.itemInstance.itemReference.equipSlot
                    && slotType != SlotType.Storage)
                {
                    return;
                }

                if (slotType == SlotType.Storage)
                {
                    uiItem.parentAfterDrag = transform;
                    uiItem.DetatchAbility();

                    return;
                }

                uiItem.parentAfterDrag = transform;
                uiItem.AppendAbility();
            }
            else
            {
                if (slotType != SlotType.Storage)
                {
                    return;
                }

                uiItem.parentAfterDrag = transform;
            }
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
        uiItem.Initialize(itemInstance, this);
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
