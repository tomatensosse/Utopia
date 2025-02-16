using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI_Item : MonoBehaviour
{
    public ItemInstance itemInstance;
    public Image icon;
    public TMP_Text amountText;

    public int slotIndex = -1;

    public void Initialize(int slotIndex, ItemInstance itemInstance = null)
    {
        this.slotIndex = slotIndex;

        if (itemInstance == null)
        {
            icon.sprite = null;
            amountText.text = "";
            return;
        }

        this.itemInstance = itemInstance;
        icon.sprite = itemInstance.itemReference.icon;
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
        icon.sprite = itemInstance.itemReference.icon;
        amountText.text = itemInstance.amount.ToString();

        itemInstance.inventorySlotIndex = slotIndex;
    }
}