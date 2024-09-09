using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class EntityCrate : Entity, IHealth, IInteractable, IHoldableObject
{
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    private bool isHeld = false;
    private Transform playerTransform;

    public override void Start()
    {
        base.Start();

        if (isServer)
        {
            MaxHealth = 100;
            CurrentHealth = MaxHealth;
        }

        if (wasLoadedBefore) { return; }

        if (entityRigidbody == null)
        {
            entityRigidbody = gameObject.AddComponent<Rigidbody>();
        }

        Vector3 randomDirection = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        entityRigidbody.AddForce(randomDirection * 20f, ForceMode.Impulse);
    }

#region IHealth
    [Command (requiresAuthority = false)]
    public void CmdTakeDamage(int damage, NetworkConnectionToClient conn)
    {
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
    public void CmdInteract(NetworkConnectionToClient conn)
    {
        Debug.Log(conn.identity + "interacted with crate!");
    }
#endregion

#region IHoldable
    public void PickUp(Transform parent)
    {
        isHeld = true;
        playerTransform = parent;
        entityRigidbody.isKinematic = true; // Disable physics
        transform.SetParent(playerTransform);
    }

    public void Drop()  
    {
        isHeld = false;
        playerTransform = null;
        entityRigidbody.isKinematic = false; // Enable physics
        transform.SetParent(null);
    }

    public void Throw(Vector3 throwForce)
    {
        if (isHeld)
        {
            Drop();
            entityRigidbody.AddForce(throwForce, ForceMode.Impulse);
        }
    }

    public void Rotate(Vector3 rotationDelta)
    {
        if (isHeld)
        {
            transform.Rotate(rotationDelta);
        }
    }

    [Command (requiresAuthority = false)]
    public void CmdPickUp(NetworkConnectionToClient conn)
    {
        RpcPickUp();
    }

    [ClientRpc (includeOwner = true)]
    public void RpcPickUp()
    {
        PickUp(playerTransform);
    }
#endregion
}
