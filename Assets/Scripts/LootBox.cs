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

    [Command(requiresAuthority = false)]
    public void OnInteract(Player player)
    {
        if (!CanInteract(player))
        {
            return;
        }
        
        Item itemInBox = GetRandomItem();
        if (itemInBox != null)
        {
            player.CmdAddItem(itemInBox);
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