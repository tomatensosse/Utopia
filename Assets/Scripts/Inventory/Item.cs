using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Required")]
    public string ItemID;
    public Sprite ItemIcon;
    public string ItemName;
    public string ItemDescription;
    public int MaxStack = -1;
    public int MaxDurability = -1;
    public bool FloorItemHoldable = true;

    [Header("Floor Item")]
    public Mesh FloorItemMesh;
    public Material FloorItemMaterial;
    public Vector3 FloorItemScale = Vector3.one;
    public Vector3 FloorItemRotation = Vector3.zero;
    public float ColliderSize = 1f;
}
