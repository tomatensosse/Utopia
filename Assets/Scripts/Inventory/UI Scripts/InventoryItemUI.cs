using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Components")]
    public Image image;
    public TMP_Text amountText;
    public GameObject durabilityDisplay;
    public Image durabilityImage;
    private int maxDurability = 100;

    [HideInInspector] public Transform parentAfterDrag;
    
    public void InitializeItem(ItemData itemData)
    {
        Item item = ItemDatabase.GetItemByID(itemData.itemID);
        image.sprite = item.ItemIcon;
        maxDurability = item.MaxDurability;

        RefreshAmount(itemData.itemAmount);
        RefreshDurability(itemData.itemDurability);
    }

    public void RefreshAmount(int amount)
    {
        amountText.text = amount.ToString();
        bool textActive = amount > 1;
        amountText.gameObject.SetActive(textActive);
    }

    public void RefreshDurability(int durability)
    {
        bool durabilityActive = durability > 0;
        durabilityDisplay.SetActive(durabilityActive);
        if (durabilityActive)
        {
            durabilityImage.fillAmount = (float)durability / maxDurability;
        }
    }

    // Move items

    public void MoveItems(int index1, int index2)
    {
        InventoryManager.Instance.MoveItem(index1, index2);
    }

    // Swap items

    public void SwapItems(int index1, int index2)
    {
        InventoryManager.Instance.SwapItems(index1, index2);
    }

    // Drag events

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
}
