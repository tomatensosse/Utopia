using UnityEngine;

public class Item : ScriptableObject
{
    [Header("Required")]
    public string ItemID;
    public Sprite ItemIcon;
    public string ItemName;
    public string ItemDescription;
}
