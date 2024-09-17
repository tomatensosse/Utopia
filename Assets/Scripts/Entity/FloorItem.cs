using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class FloorItem : Entity
{
    // Variables
    public ItemData itemData;
    private Item item;

    // References
    public BoxCollider itemCollider;
    public MeshFilter itemMeshFilter;
    public MeshRenderer itemMeshRenderer;
    public Transform display;

    public override void Awake()
    {
        // Empty
    }

    public override void Start()
    {
        // Empty
    }

    public void Initialize(ItemData itemData)
    {
        if (rb == null || itemCollider == null || itemMeshFilter == null || itemMeshRenderer == null)
        {
            Debug.LogError("Bora155 | Reference is null");
        }

        item = ItemDatabase.GetItemByID(itemData.itemID);
        
        this.entityName = item.ItemName;
        this.entityDescription = item.ItemDescription;
        this.itemData = itemData; // includes itemID amount and durability

        this.itemMeshFilter.mesh = item.FloorItemMesh;
        this.itemMeshRenderer.material = item.FloorItemMaterial;

        this.itemCollider.size = new Vector3(item.ColliderSize, item.ColliderSize, item.ColliderSize);

        display.localScale = item.FloorItemScale * 100;
        display.rotation = Quaternion.Euler(item.FloorItemRotation);

        SetDefaults();
    }

    public override void SetDefaults()
    {
        entityID = "entity_floor_item";

        hasHealth = false;
        isInteractable = true;
        isHoldable = item.FloorItemHoldable;
    }

    public override void CmdInteract(NetworkConnectionToClient conn)
    {
        base.CmdInteract(conn);

        Inventory playerInventory = conn.identity.GetComponent<Inventory>();

        if (playerInventory.Add(item, itemData.itemAmount, itemData.itemDurability))
        {
            NetworkServer.Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Failed to add item to inventory.");
        }
    }

    public override EntityData SaveEntity()
    {
        FloorItemData data = new FloorItemData();
        data.entityID = entityID;
        data.itemID = item.ItemID;
        data.itemAmount = itemData.itemAmount;
        data.itemDurability = itemData.itemDurability;

        data.position = transform.position;
        data.rotation = transform.rotation;

        return data;
    }

    public override void LoadEntity(EntityData entityData)
    {
        FloorItemData data = (FloorItemData)entityData;

        item = ItemDatabase.GetItemByID(data.itemID);

        entityID = data.entityID;

        itemData = new ItemData
        {
            itemID = data.itemID,
            itemAmount = data.itemAmount,
            itemDurability = data.itemDurability
        };

        transform.position = data.position;
        transform.rotation = data.rotation;

        SetDefaults();
    }
}

[System.Serializable]
public class FloorItemData : EntityData // For saving/loading; Floor items will have 2 extra fields: itemID and amount
{
    public string itemID;
    public int itemAmount;
    public int itemDurability;
}
