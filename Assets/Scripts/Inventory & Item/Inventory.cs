using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Inventory : NetworkBehaviour
{
    public Item heldItem;
    private Item oldItem;
    public int inventorySize = 32; // 8 hotbar slots, 24 inventory slots
    public ItemData[] inventory;
    public Player player;
    public InventoryManager inventoryManager;
    public ItemHandler itemHandler;
    private ItemDatabase itemDatabase;

    public void Initialize()
    {
        if (!LoadInventory())
        {
            inventory = new ItemData[inventorySize];
        }

        if (player == null)
        {
            player = GetComponent<Player>();
        }

        if (inventoryManager == null)
        {
            inventoryManager = GetComponent<InventoryManager>();
        }

        if (itemDatabase == null)
        {
            itemDatabase = ItemDatabase.Instance;
        }

        Debug.Log("Inventory initialized with " + inventorySize + " slots.");

        Push(true, 0);
    }

    public void Push(bool holdItemOnPush = false, int index = -1)
    {
        CmdInventoryUpdated(inventory);

        if (holdItemOnPush)
        {
            Hold(index);
        }
    }

    public int GetCountOfSlot(int slotIndex)
    {
        return inventory[slotIndex].itemAmount;
    }

    public void SetCountOfSlot(int slotIndex, int newCount)
    {
        inventory[slotIndex].itemAmount = newCount;
    }

    [Command(requiresAuthority = false)]
    public void CmdInventoryUpdated(ItemData[] newInventory)
    {
        Debug.Log("Cmd | Inventory updated.");
        inventory = newInventory;
        RpcInventoryUpdated(newInventory);
    }

    [ClientRpc(includeOwner = false)]
    public void RpcInventoryUpdated(ItemData[] newInventory)
    {
        Debug.Log("Rpc | Inventory updated.");
        inventory = newInventory;
    }

    public void Hold(int index)
    {
        oldItem = heldItem;

        if (index < 0 || index >= inventorySize)
        {
            Debug.LogError("Invalid index to hold item.");
            return;
        }

        if (inventory[index] == null || string.IsNullOrEmpty(inventory[index].itemID))
        {
            Debug.Log("No item in slot " + index + " to hold.");
            heldItem = null;

            if (oldItem == null && heldItem == null)
            {
                Debug.Log("No rpc.");
                return;
            }

            CmdHoldItem(null);
            return;
        }

        Item newHeldItem = itemDatabase.GetItemByID(inventory[index].itemID);;
        heldItem = newHeldItem;
        
        CmdHoldItem(newHeldItem.itemID);
    }

    [Command(requiresAuthority = false)]
    public void CmdHoldItem(string itemID)
    {
        Debug.Log("Cmd | Holding item with ID: " + itemID);
        if (string.IsNullOrEmpty(itemID))
        {
            heldItem = null;
            RpcHoldItem(null);
        }
        heldItem = itemDatabase.GetItemByID(itemID);
        RpcHoldItem(itemID);

        if (!string.IsNullOrEmpty(itemID))
        {
            itemHandler.SpawnItemServerRpc(connectionToClient, heldItem.itemID);
        }
    }

    [ClientRpc(includeOwner = false)]
    public void RpcHoldItem(string itemID)
    {
        Debug.Log("Rpc | Holding item with ID: " + itemID);
        if (string.IsNullOrEmpty(itemID))
        {
            heldItem = null;
            return;
        }
        heldItem = itemDatabase.GetItemByID(itemID);
    }

#region Loading and Saving

    public void SetInventory(ItemData[] setInventory)
    {
        if (setInventory.Length != inventorySize)
        {
            Debug.Log("Inventory size mismatch | Resetting inventory...");
            inventory = new ItemData[inventorySize];
        }
        else
        {
            inventory = setInventory;
        }
    }
    public bool LoadInventory()
    {
        if (inventory == null)
        {
            Debug.Log("False");
            return false;
        }

        inventoryManager.LoadItemsFromInventory();

        Debug.Log("True");
        return true;
    }

#endregion
}
