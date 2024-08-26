using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class ItemHandler : NetworkBehaviour
{
    public Transform localItemContainer;
    public Transform remoteItemContainer;
    public PlayerViewmodel playerViewmodel;

    private void Awake()
    {
        playerViewmodel = PlayerViewmodel.Instance;
    }

    public void Initialize()
    {
        if (localItemContainer == null)
        {
            localItemContainer = GameObject.Find("LocalItemContainer").transform;
        }

        if (remoteItemContainer == null)
        {
            remoteItemContainer = GameObject.Find("RemoteItemContainer").transform;
        }

        if (playerViewmodel == null)
        {
            playerViewmodel = GameObject.Find("PlayerViewModel").GetComponent<PlayerViewmodel>();
        }
    }

    [Command(requiresAuthority = false)]
    public void SpawnItemServerRpc(NetworkConnectionToClient conn, string itemID)
    {
        Item currentItem = ItemDatabase.Instance.GetItemByID(itemID);
        Debug.Log("Spawning item: " + currentItem.itemName);
        GameObject itemInstance = Instantiate(currentItem.itemObject, remoteItemContainer.position, remoteItemContainer.rotation);

        NetworkServer.Spawn(itemInstance, conn);

        RpcSpawnItemClientRpc(itemInstance);
    }

    [ClientRpc(includeOwner = true)]
    void RpcSpawnItemClientRpc(GameObject itemInstance)
    {
        ItemObject itemObject = itemInstance.GetComponent<ItemObject>();

        if (!isLocalPlayer)
        {
            itemInstance.transform.SetParent(remoteItemContainer);
        }
        else
        {
            itemInstance.transform.SetParent(localItemContainer);

            playerViewmodel.objectToHold = itemInstance.transform;
            playerViewmodel.AdjustFingerTargets(itemObject.itemHoldPoints);
            playerViewmodel.Wield();
        }
    }
}
