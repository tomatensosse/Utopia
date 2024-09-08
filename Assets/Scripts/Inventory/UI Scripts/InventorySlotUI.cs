using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IDropHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;

    private void Awake()
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
            InventoryItemUI draggedItem = eventData.pointerDrag.GetComponent<InventoryItemUI>();
            draggedItem.MoveItems(draggedItem.parentAfterDrag.GetSiblingIndex(), transform.GetSiblingIndex());

            draggedItem.parentAfterDrag = transform;
            draggedItem.transform.SetParent(transform);
        }
        else if (transform.childCount == 1)
        {
            Transform currentChild = transform.GetChild(0);

            InventoryItemUI draggedItem = eventData.pointerDrag.GetComponent<InventoryItemUI>();
            draggedItem.SwapItems(draggedItem.parentAfterDrag.GetSiblingIndex(), transform.GetSiblingIndex());

            currentChild.SetParent(draggedItem.parentAfterDrag, false);
            draggedItem.parentAfterDrag = transform;
        }
    }
}
