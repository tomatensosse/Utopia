using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI_Slot : MonoBehaviour
{
    public InventoryUI_Slot inventoryUISlot;
    public KeyCode hotkey;
    public GameHotbarSlotType slotType;

    public TMP_Text hotkeyText;

    public Image propItemImage;
    public TMP_Text propItemAmountText;

    public void Initialize() // Change to initialize and initialize on player.cs
    {
        inventoryUISlot.gameUISlot = this;

        string hotkeyString = hotkey.ToString();

        if (hotkeyString.Contains("Alpha"))
        {
            hotkeyString = hotkeyString.Replace("Alpha", "");
        }

        hotkeyText.text = hotkeyString; 

        ClearSlot(); 
    }

    public void UpdateSlot(ItemInstance itemInstance)
    {
        propItemImage.color = new Color(1, 1, 1, 1);
        propItemImage.sprite = itemInstance.itemReference.icon;

        if (itemInstance.itemReference.isStackable)
        {
            propItemAmountText.gameObject.SetActive(true);
            propItemAmountText.text = itemInstance.amount.ToString();
        }
        else
        {
            propItemAmountText.gameObject.SetActive(false);
        }
    }

    public void UpdateAmount(int newAmount)
    {
        propItemAmountText.text = newAmount.ToString();
    }

    public void ClearSlot()
    {
        propItemImage.color = new Color(0, 0, 0, 0);
        propItemImage.sprite = null;
        propItemAmountText.text = "";
    }
}