using System;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// IF GENERATING WITHOUT CONSTRUCTOR, SET GUID MANUALLY !!!
/// </summary>
[Serializable]
public struct ItemInstance
{
    [NonSerialized] public Item itemReference;
    public string instanceUID;
    public string itemReferenceUID;
    public int amount;

    public ItemInstance(string itemReferenceUID, int amount)
    {
        instanceUID = Guid.NewGuid().ToString();

        itemReference = null; // Will be generated in Deserialize()
        this.itemReferenceUID = itemReferenceUID;
        this.amount = amount;
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
}