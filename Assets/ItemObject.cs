using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ItemObject : Interactable
{
    // Also going to work as an interactable.cs

    public Collider itemCollider;
    public Item item;
    public DynamicItem dynamicItem; // TBA
    [Header("Finger Tip Targets // ASSIGN CAREFULLY")]
    public Transform[] itemHoldPoints;

    public virtual void Start()
    {
        itemCollider = GetComponent<Collider>();
    }

    public override void Interact(uint whoIsInteracting)
    {
        if (NetworkClient.spawned.TryGetValue(whoIsInteracting, out NetworkIdentity identity))
        {
            Player player = identity.GetComponent<Player>();
            if (player != null)
            {
                string playerName = player.playerState.playerSaveName;
                Debug.Log(playerName + " is picking up " + this.name);

                player.playerInventoryManager.AddItem(item);
                NetworkServer.Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("PlayerObjectController component not found on the interacting object.");
            }
        }
        else
        {
            Debug.LogWarning("Interacting player not found in NetworkServer.spawned.");
        }
    }
}
