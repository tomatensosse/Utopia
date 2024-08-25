using UnityEngine;

[System.Serializable]
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
    public int itemCurrentDurability = 10;
}
