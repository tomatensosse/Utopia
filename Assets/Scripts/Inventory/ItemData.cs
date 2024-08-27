using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[System.Serializable]
public class ItemData : ISerializationCallbackReceiver
{
    public string itemID;
    [NonSerialized] private int _itemAmount = 1;
    // private backing field
    [NonSerialized] private int _durability = -1;

    // property to access itemAmount
    public int itemAmount
    {
        get => _itemAmount;
        set => _itemAmount = value;
    }

    [SerializeField]
    private bool _shouldSerializeItemAmount = false;

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
            itemAmount = itemAmount > 0 ? itemAmount : 1,
            durability = durability > -1 ? durability : -1
        };

        generated._shouldSerializeItemAmount = itemAmount > -1;
        generated._shouldSerializeDurability = durability > -1;

        return generated;
    }

    public string ItemToJson()
    {
        // Start JSON with opening brace
        string json = "{";

        // Add itemID and itemAmount to JSON string
        json += $"\"itemID\":\"{itemID}\"";
        
        // Conditionally add itemAmount if needed
        if (_shouldSerializeItemAmount && itemAmount != 1)
        {
            json += $",\"itemAmount\":{itemAmount}";
        }

        // Conditionally add durability if needed
        if (_shouldSerializeDurability && durability != -1)
        {
            json += $",\"durability\":{durability}";
        }

        // Continue adding JSON string with "," if needed

        // Close JSON with ending brace
        json += "}";

        return json;
    }

    public ItemData JsonToItem(string json)
    {
        ItemData itemData = new ItemData();

        JsonUtility.FromJsonOverwrite(json, itemData);

        itemData._shouldSerializeItemAmount = itemData.itemAmount != 1;
        itemData._shouldSerializeDurability = itemData.durability != -1;

        return itemData;
    }

    public void OnBeforeSerialize()
    {
        if (!_shouldSerializeItemAmount)
        {
            _itemAmount = 1;
        }
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
