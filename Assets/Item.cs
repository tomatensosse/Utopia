using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string itemID;
    public int itemAmount;
    public int itemDurability;
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Item")]
public class Item : ScriptableObject
{
    public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary, }

    [Header("Required")]
    public string itemID;
    public string itemName;
    public string itemDescription;
    public ItemRarity itemRarity;
    public GameObject itemObject;
    public Sprite itemIcon;
    public bool isStackable = false;
    [ConditionalHide("isStackable", true)]
    public int itemMaxStack = 99;

    [Header("Optional")]
    public int itemValue = 0;
    public int itemMaxDurability = 10;
}
