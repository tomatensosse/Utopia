using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;

    public void Awake()
    {
        Deselect();
    }

    public void Select()
    {
        image.color = selectedColor;
    }

    public void Deselect()
    {
        image.color = notSelectedColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            InventorySlotItem inventoryItem = eventData.pointerDrag.GetComponent<InventorySlotItem>();
            inventoryItem.MoveItem(inventoryItem.parentAfterDrag.GetSiblingIndex(), transform.GetSiblingIndex());

            inventoryItem.parentAfterDrag = transform;
        }
        else if (transform.childCount == 1)
        {
            Transform currentChild = transform.GetChild(0);

            InventorySlotItem inventoryItem = eventData.pointerDrag.GetComponent<InventorySlotItem>();
            inventoryItem.SwapItems(inventoryItem.parentAfterDrag.GetSiblingIndex(), transform.GetSiblingIndex());

            currentChild.SetParent(inventoryItem.parentAfterDrag, false);
            inventoryItem.parentAfterDrag = transform;
        }
    }
}
