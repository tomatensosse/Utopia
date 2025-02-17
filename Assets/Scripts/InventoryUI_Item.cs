using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI_Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemInstance itemInstance;
    public Image image;
    public TMP_Text amountText;

    public int slotIndex = -1;

    #region Drag and Drop

    [HideInInspector] public Transform parentAfterDrag;

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

    #endregion

    public void Initialize(int slotIndex, ItemInstance itemInstance = null)
    {
        this.slotIndex = slotIndex;

        if (itemInstance == null)
        {
            image.sprite = null;
            amountText.text = "";
            return;
        }

        this.itemInstance = itemInstance;
        image.sprite = itemInstance.itemReference.icon;
        amountText.text = itemInstance.amount.ToString();

        itemInstance.inventorySlotIndex = slotIndex;
    }

    public void Initialize(ItemInstance itemInstance)
    {
        if (slotIndex == -1)
        {
            Debug.LogError("Slot index not set for InventoryUI_Item");
            return;
        }

        this.itemInstance = itemInstance;
        image.sprite = itemInstance.itemReference.icon;
        amountText.text = itemInstance.amount.ToString();

        itemInstance.inventorySlotIndex = slotIndex;
    }

    public void UpdateAmount(int oldAmount, int newAmount)
    {
        amountText.text = newAmount.ToString();
    }
}