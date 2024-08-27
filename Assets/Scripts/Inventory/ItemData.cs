using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[System.Serializable]
public class ItemData : ISerializationCallbackReceiver
{
    public string itemID;
    public int itemAmount;
    // private backing field
    [NonSerialized] private int _durability = -1;

    // property to access durability
    public int durability
    {
        get => _durability;
        set => _durability = value;
    }

    [SerializeField]
    private bool _shouldSerializeDurability = false;

    public ItemData Generate(string itemID, int itemAmount = -1, int durability = -1)
    {
        ItemData generated = new ItemData
        {
            itemID = itemID,
            itemAmount = itemAmount > -1 ? itemAmount : this.itemAmount,
            durability = durability > -1 ? durability : -1
        };

        generated._shouldSerializeDurability = durability > -1;

        return generated;
    }

    public string ItemToJson()
    {
        // Start JSON with opening brace
        string json = "{";

        // Add itemID and itemAmount to JSON string
        json += $"\"itemID\":\"{itemID}\",";
        json += $"\"itemAmount\":{itemAmount}";

        // Conditionally add durability if needed
        if (_shouldSerializeDurability && durability != -1)
        {
            json += $",\"durability\":{durability}";
        }

        // Close JSON with ending brace
        json += "}";

        return json;
    }

    public ItemData JsonToItem(string json)
    {
        ItemData itemData = new ItemData();

        JsonUtility.FromJsonOverwrite(json, itemData);

        itemData._shouldSerializeDurability = itemData.durability != -1;

        return itemData;
    }

    public void OnBeforeSerialize()
    {
        if (!_shouldSerializeDurability)
        {
            _durability = -1;
        }
    }

    public void OnAfterDeserialize()
    {
        // Deserialize logic if needed (bs)
    }
}
