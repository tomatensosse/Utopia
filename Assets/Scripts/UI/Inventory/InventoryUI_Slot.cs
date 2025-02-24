using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI_Slot : MonoBehaviour, IDropHandler
{
    public InventorySlotType slotType;
    public int slotIndex = -1;
    public InventoryUI_Item uiItem; // When UI Item Changes, Check for abilities...

    public GameUI_Slot gameUISlot = null;

    #region Drag and Drop

    public void OnDrop(PointerEventData eventData)
    {
        InventoryUI_Item draggedUiItem = eventData.pointerDrag.GetComponent<InventoryUI_Item>();

        if (uiItem == null) // If dragged slot is empty
        {
            if (draggedUiItem.itemInstance.itemReference.isEquippable)
            {
                if (slotType != draggedUiItem.itemInstance.itemReference.equipSlot
                    && slotType != InventorySlotType.Storage) // Equip dragged to wrong equip slot
                {
                    return;
                }

                // Maybe combine these methods ???

                if (slotType == InventorySlotType.Storage) // If equip dragged to storage slot:
                {
                    draggedUiItem.CurrentSlot.gameUISlot?.ClearSlot(); // Update hotbar slot

                    draggedUiItem.parentAfterDrag = transform;
                    draggedUiItem.CurrentSlot.uiItem = null; // Set old slot item to null
                    draggedUiItem.CurrentSlot = this; // Set new slot as this
                    uiItem = draggedUiItem; // Set new slot item to dragged item

                    gameUISlot?.UpdateSlot(uiItem.itemInstance); // Update hotbar slot
                    return;
                }

                if (slotType == draggedUiItem.itemInstance.itemReference.equipSlot) // If equip dragged to equip slot:
                {
                    draggedUiItem.CurrentSlot.gameUISlot?.ClearSlot(); // Update hotbar slot

                    draggedUiItem.parentAfterDrag = transform;
                    draggedUiItem.CurrentSlot.uiItem = null; // Set old slot item to null
                    draggedUiItem.CurrentSlot = this; // Set new slot as this
                    uiItem = draggedUiItem; // Set new slot item to dragged item
                    Debug.Log($"EQUIP {draggedUiItem.itemInstance.itemReference.itemName}");

                    gameUISlot?.UpdateSlot(uiItem.itemInstance); // Update hotbar slot
                    return;
                }
            }
            else // If not equippable, transfer as long as slot is storage.
            {
                if (slotType != InventorySlotType.Storage)
                {
                    return;
                }

                draggedUiItem.CurrentSlot.gameUISlot?.ClearSlot(); // Update hotbar slot

                draggedUiItem.parentAfterDrag = transform;
                draggedUiItem.CurrentSlot.uiItem = null; // Set old slot item to null
                draggedUiItem.CurrentSlot = this; // Set new slot as this
                uiItem = draggedUiItem; // Set new slot item to dragged item

                gameUISlot?.UpdateSlot(uiItem.itemInstance); // Update hotbar slot
                return;
            }
        }

        if (uiItem != null) // If dragged slot is not empty
        {
            Debug.Log("Swapping not yet implemented.");
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

        gameUISlot?.UpdateSlot(uiItem.itemInstance); // Update hotbar slot
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
