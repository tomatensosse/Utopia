
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [Header("UI")]
    public Image image;
    public TMP_Text countText;
    private InventoryManager inventoryManager;

    [HideInInspector] public Transform parentAfterDrag;
    public Item item;
    [HideInInspector] public int count = 1;

    public void InitialiseItem(Item newItem, InventoryManager inventoryManagerToLink)
    {
        item = newItem;
        image.sprite = newItem.itemIcon;
        inventoryManager = inventoryManagerToLink;
        RefreshCount();
    }

    public void RefreshCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }

    public void SwapItems(int index1, int index2)
    {
        inventoryManager.SwapItems(index1, index2);
    }

    public void MoveItem(int fromIndex, int toIndex)
    {
        inventoryManager.MoveItem(fromIndex, toIndex);
    }
}
