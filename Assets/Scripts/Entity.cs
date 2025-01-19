using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public abstract class Entity : NetworkBehaviour
{
    [Header("Network Properties")]
    [SyncVar]
    protected int entityId;
    
    protected bool isInitialized = false;

    protected NetworkTransformReliable nt;
    protected Rigidbody rb;

    public virtual void Awake()
    {
        nt = GetComponent<NetworkTransformReliable>();
        rb = GetComponent<Rigidbody>();
    }

    #region Mirror Lifecycle Methods

    public override void OnStartServer()
    {
        base.OnStartServer();
        ServerInitialize();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        ClientInitialize();
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        AuthorityInitialize();
    }

    protected virtual void ServerInitialize()
    {
        // Server Initialize runs only on the server
    }

    protected virtual void ClientInitialize()
    {
        // Client Initialize runs only on the client
    }

    protected virtual void AuthorityInitialize()
    {
        // Authority Initialize runs only on the client with authority
    }

    public virtual void Initialize()
    {
        if (isServer)
        {
            // Assign a unique entity ID
        }

        isInitialized = true;
    }

    #endregion

    #region Update Methods

    protected virtual void Update()
    {
        if (isServer)
        {
            ServerUpdate();
        }

        if (isClient)
        {
            ClientUpdate();
        }

        // Local Update runs both on single player and multi player
        LocalUpdate();
    }

    protected virtual void ServerUpdate()
    {
        // Server Update runs only on the server
    }

    protected virtual void ClientUpdate()
    {
        // Client Update runs only on the client
    }

    protected virtual void LocalUpdate()
    {
        // Local Update runs on both single player and multi player
    }

    protected virtual void FixedUpdate()
    {
        if (isServer)
        {
            ServerFixedUpdate();
        }

        if (isClient)
        {
            ClientFixedUpdate();
        }

        // Local Fixed Update runs both on single player and multi player
        LocalFixedUpdate();
    }

    protected virtual void ServerFixedUpdate()
    {
        // Server Fixed Update runs only on the server
    }

    protected virtual void ClientFixedUpdate()
    {
        // Client Fixed Update runs only on the client
    }

    protected virtual void LocalFixedUpdate()
    {
        // Local Fixed Update runs on both single player and multi player
    }

    #endregion

    #region Utility Methods

    protected bool IsSinglePlayer()
    {
        return false; //!GameManager.SessionSettings.online;
    }

    protected bool IsOwnerOrSinglePlayer()
    {
        return IsSinglePlayer() || isOwned;
    }

    public int GetEntityId()
    {
        return entityId;
    }

    #endregion

    #region Rigidbody Method Acessors

    public void ApplyForce(Vector3 force, ForceMode forceMode)
    {
        rb.AddForce(force, forceMode);
    }

    public void SetVelocity(Vector3 velocity)
    {
        rb.velocity = velocity;
    }

    #endregion
}