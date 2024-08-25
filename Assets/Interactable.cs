using UnityEngine;
using Mirror;

public class Interactable : NetworkBehaviour
{
    [Command(requiresAuthority = false)]
    public virtual void CmdInteract(uint whoIsInteracting)
    {
        Debug.Log("CmdInteract called by " + whoIsInteracting);
        RpcInteract(whoIsInteracting);
    }

    [ClientRpc]
    public virtual void RpcInteract(uint whoIsInteracting)
    {
        Debug.Log("RpcInteract called by " + whoIsInteracting);
        Interact(whoIsInteracting);
    }

    public virtual void Interact(uint whoIsInteracting)
    {
        if (NetworkClient.spawned.TryGetValue(whoIsInteracting, out NetworkIdentity identity))
        {
            Player player = identity.GetComponent<Player>();
            if (player != null)
            {
                string playerName = player.playerState.playerSaveName;
                Debug.Log(playerName + " is interacting with " + this.name);
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
