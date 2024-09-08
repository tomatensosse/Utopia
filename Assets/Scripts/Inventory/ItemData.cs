using System;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class ItemData
{
    public string itemID;

    // private (set to public for debug) backing fields
    [NonSerialized] public int _itemAmount = 1;
    [NonSerialized] public int _itemDurability = -1;

    // public accessors & mutators
    public int itemAmount
    {
        get => _itemAmount;
        set => _itemAmount = value;
    }
    [SerializeField] bool _shouldSerializeItemAmount;

    public int itemDurability
    {
        get => _itemDurability;
        set => _itemDurability = value;
    }
    [SerializeField] bool _shouldSerializeItemDurability;

    public static ItemData Generate(string itemID, int itemAmount = -1, int itemDurability = -1)
    {
        ItemData generated = new ItemData
        {
            itemID = itemID,
            itemAmount = itemAmount > 0 ? itemAmount : 1,
            itemDurability = itemDurability > 0 ? itemDurability : -1
        };

        generated._shouldSerializeItemAmount = itemAmount > -1;
        generated._shouldSerializeItemDurability = itemDurability > -1;

        return generated;
    }

    public string ItemToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public static ItemData JsonToItem(string json)
    {
        return JsonConvert.DeserializeObject<ItemData>(json);
    }
}
