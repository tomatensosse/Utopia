using Mirror;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    [System.NonSerialized] public Item itemReference;
    public string itemReferenceUID;
    public int inventorySlotIndex;
    public int amount;

    public ItemInstance(string itemReferenceUID, int amount, int inventorySlotIndex)
    {
        this.itemReferenceUID = itemReferenceUID;
        this.amount = amount;
        this.inventorySlotIndex = inventorySlotIndex;
    }

    public void Deserialize()
    {
        itemReference = ItemDatabase.Instance.GetItem(itemReferenceUID);

        if (itemReference != null)
        {
            Debug.Log("Success!");
        }
        else
        {
            Debug.LogError("Failed to deserialize ItemInstance.");
        }
    }

    public ItemInstance()
    {
        itemReference = null;
        itemReferenceUID = "";
        amount = 0;
        inventorySlotIndex = -1;
    }
}