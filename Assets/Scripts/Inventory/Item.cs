using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Required")]
    public string ItemID;
    public Sprite ItemIcon;
    public string ItemName;
    public string ItemDescription;
}
