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
        InventoryItemUI draggedItem = eventData.pointerDrag.GetComponent<InventoryItemUI>();

        int from = InventoryManager.Instance.ReturnIndexOf(draggedItem.parentAfterDrag.GetComponent<InventorySlotUI>());
        int to = InventoryManager.Instance.ReturnIndexOf(this);

        if (transform.childCount == 0)
        {
            draggedItem.MoveItems(from, to);

            draggedItem.parentAfterDrag = transform;
        }
        else if (transform.childCount == 1)
        {
            Transform currentChild = transform.GetChild(0);

            draggedItem.SwapItems(from, to);

            currentChild.SetParent(draggedItem.parentAfterDrag, false);
            draggedItem.parentAfterDrag = transform;
        }
    }
}
