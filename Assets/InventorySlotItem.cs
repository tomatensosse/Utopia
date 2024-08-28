using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image image;
    public TMP_Text countText;
    public GameObject durabilityDisplay;
    public Image durabilityImage;

    [Header("Data")]
    public Item item;
    public ItemData itemData;
    private InventoryManager inventoryManager;

    // Stuff for drag and drop and params
    [HideInInspector] public Transform parentAfterDrag;

    private void Awake()
    {
        inventoryManager = InventoryManager.Instance;
    }

    public void InitializeItem(Item item, ItemData itemData)
    {
        // Set item and params
        this.item = item;
        image.sprite = item.ItemIcon;

        // Set itemData and params
        this.itemData = itemData;

        // Refresh UI
        RefreshCount();
        RefreshDurability();
    }

    public void RefreshCount()
    {
        countText.text = itemData.itemAmount.ToString();
        bool textActive = itemData.itemAmount > 1;
        countText.gameObject.SetActive(textActive);
    }

    public void RefreshDurability()
    {
        float durabilityPercentage = (float)itemData.durability / item.MaxDurability;
        durabilityImage.fillAmount = durabilityPercentage;

        bool durabilityActive = itemData.durability > 0;
        durabilityDisplay.SetActive(durabilityActive);
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
        Debug.Log("Swapping items |" + index1 + "|" + index2);
        //inventoryManager.SwapItems(index1, index2);
    }

    public void MoveItem(int fromIndex, int toIndex)
    {
        Debug.Log("Moving item |" + fromIndex + "|" + toIndex);
        //inventoryManager.MoveItem(fromIndex, toIndex);
    }
}
