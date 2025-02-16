using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class LootBox : Entity, IInteractable
{
    [Header("Interaction Settings")]
    public List<Item> lootTable = new List<Item>();

    public bool CanInteract(Player player)
    {
        // Any flags here
        return true;
    }

    public void OnInteract(Player player)
    {
        CmdHandleInteraction(player);
    }

    [Command(requiresAuthority = false)]
    public void CmdHandleInteraction(Player player)
    {
        if (!CanInteract(player))
        {
            return;
        }

        Item itemInBox = GetRandomItem();
        if (itemInBox != null)
        {
            if (itemInBox.isStackable)
            {
                int randomAmount = Random.Range(1, itemInBox.maxStack);
                
                player.AddToInventory(itemInBox.uid, randomAmount);
            }
        }
    }

    private Item GetRandomItem()
    {
        if (lootTable.Count > 0)
        {
            return lootTable[Random.Range(0, lootTable.Count)];
        }

        Debug.LogWarning("Loot table is empty!");
        return null;
    }
}