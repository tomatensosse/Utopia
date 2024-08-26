using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class ItemHandler : NetworkBehaviour
{
    public Transform itemContainer;
    public PlayerAnimation playerAnimation;

    public void Initialize()
    {
        // do sum shit
    }

    [Command(requiresAuthority = false)]
    public void SpawnItemServerRpc(NetworkConnectionToClient conn, string itemID)
    {
        Item currentItem = ItemDatabase.Instance.GetItemByID(itemID);
        Debug.Log("Spawning item: " + currentItem.itemName);
        GameObject itemInstance = Instantiate(currentItem.itemObject, itemContainer.position, itemContainer.rotation);

        NetworkServer.Spawn(itemInstance, conn);

        RpcSpawnItemClientRpc(itemInstance);
    }

    [ClientRpc(includeOwner = true)]
    void RpcSpawnItemClientRpc(GameObject itemInstance)
    {
        ItemObject itemObject = itemInstance.GetComponent<ItemObject>();

        if (isLocalPlayer)
        {
            itemInstance.transform.SetParent(itemContainer);
            playerAnimation.TriggerWield();
        }
    }
}
