using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class Entity : NetworkBehaviour, IHealth, IInteractable, IHoldableObject
{
#region Base Variables
    public string entityID; // For saving/loading and entity database
    public string entityName;
    public string entityDescription;
    public EntityData entityData;
    [HideInInspector] public bool wasLoadedBefore; // to prevent double loading
    public bool initialized;
    public Rigidbody rb;
#endregion

#region Defaults
    public bool hasHealth = true;
    [ConditionalHide(nameof(hasHealth), true)]
    public int maxHealth = 100;
    public bool isInteractable = true;
    public bool isHoldable = true;
#endregion

#region IHealth
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
#endregion

#region IInteractable && IHoldableObject
    public bool IsHeld { get; set; }
    public bool OnCooldown { get; set; }
    private Transform swingPoint;
#endregion

    public virtual void Awake()
    {
        SetDefaults();
    }

    public virtual void Start()
    {
        initialized = true;

        if (isServer)
        {
            MaxHealth = 100; // FIXME: Load from data
            CurrentHealth = MaxHealth;
        }

        if (wasLoadedBefore) { return; }

        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
    }

    public virtual void SetDefaults() // Called on Awake before everything else
    {
        // Set stuff here
    }

#region Network and Saving
    public virtual EntityData SaveEntity()
    {
        EntityData entityData = new EntityData();
        entityData.entityID = entityID;
        entityData.position = transform.position;
        entityData.rotation = transform.rotation;

        return entityData;
    }

    public virtual void LoadEntity(EntityData entityData)
    {
        this.entityData = entityData;
        transform.position = entityData.position;
        transform.rotation = entityData.rotation;

        wasLoadedBefore = true;
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
    public void RpcGrantedAuthority(NetworkConnection conn)
    {
        // Do something with the client that was granted authority
        rb.isKinematic = false;
        Debug.Log("Authority Granted To New Client");
    }
#endregion

#region IHealth
    [Command (requiresAuthority = false)]
    public void CmdTakeDamage(int damage, NetworkConnectionToClient conn)
    {
        if (!hasHealth) { return; }

        if (isServer)
        {
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                Die(conn);
            }
        }

        RpcUpdateHealth(CurrentHealth);
    }

    [ClientRpc]
    public void RpcUpdateHealth(int health)
    {
        CurrentHealth = health;
    }

    public bool IsDead()
    {
        return CurrentHealth <= 0;
    }

    public void Die(NetworkConnectionToClient conn)
    {
        // Maybe add some loot dropping logic here
        Debug.Log("Crate destroyed!");
        NetworkServer.Destroy(gameObject);
    }
#endregion

#region IInteractable
    [Command (requiresAuthority = false)]
    public virtual void CmdInteract(NetworkConnectionToClient conn)
    {
        if (!isInteractable) { return; }

        Debug.Log(conn.identity + "interacted with " + entityName + "!");
    }
#endregion

#region IHoldable
    public void Hold(Transform parent)
    {
        if (OnCooldown) { return; }
        
        IsHeld = true;
        
        ConfigurableJoint joint = this.AddComponent<ConfigurableJoint>();
        joint.connectedBody = parent.GetComponent<Rigidbody>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = Vector3.zero;
    }

    public void UnHold()
    {
        IsHeld = false;

        ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
        Destroy(joint);
    }

    public void Throw(Vector3 throwForce)
    {
        UnHold();
        HoldCooldown();
    }

    public IEnumerator HoldCooldown()
    {
        OnCooldown = true;
        yield return new WaitForSeconds(0.5f);
        OnCooldown = false;
    }

    public void Rotate(Vector3 rotationDelta)
    {
        if (IsHeld)
        {
            transform.Rotate(rotationDelta);
        }
    }
#endregion
}

[System.Serializable]
public class EntityData
{
    public string entityID;
    public Vector3 position;
    public Quaternion rotation;
}
