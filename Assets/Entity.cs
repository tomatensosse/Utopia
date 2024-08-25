using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[System.Serializable]
public class EntityData
{
    public string entityID;
    public Vector3 position;
    public Quaternion rotation;
}

public class Entity : NetworkBehaviour
{
    public string entityID;
    public EntityData entityData;
    [HideInInspector] public bool isLoaded;
    public Rigidbody entityRigidbody; 

    public virtual void Awake()
    {
        entityRigidbody = GetComponent<Rigidbody>();
    }

    public virtual void Start()
    {
        if (isLoaded) { return; }
    }

    public EntityData SaveEntity()
    {
        EntityData entityData = new EntityData();
        entityData.entityID = entityID;
        entityData.position = transform.position;
        entityData.rotation = transform.rotation;

        return entityData;
    }

    public void LoadEntity(EntityData entityData)
    {
        this.entityData = entityData;
        transform.position = entityData.position;
        transform.rotation = entityData.rotation;

        isLoaded = true;
    }

    [Server]
    public void GrantAuthority(NetworkConnectionToClient conn)
    {
        // Make sure the current object has a NetworkIdentity component
        NetworkIdentity networkIdentity = GetComponent<NetworkIdentity>();

        // Remove authority from previous client, if any
        if (networkIdentity.connectionToClient != null)
        {
            networkIdentity.RemoveClientAuthority();
        }

        // Assign authority to the requesting client
        networkIdentity.AssignClientAuthority(conn);
        RpcGrantedAuthority(conn);
    }

    [Command(requiresAuthority = false)]
    public void CmdRequestAuthority(NetworkConnectionToClient conn)
    {
        GrantAuthority(conn);
    }

    [TargetRpc]
    public void RpcGrantedAuthority(NetworkConnectionToClient conn)
    {
        Debug.Log("Authority granted to client: " + conn.connectionId);

        // Do something with the client that was granted authority
        entityRigidbody.isKinematic = false;
    }
}
