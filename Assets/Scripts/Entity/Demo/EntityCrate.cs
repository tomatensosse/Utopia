using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class EntityCrate : Entity
{
    public override void Start()
    {
        base.Start();

        Vector3 randomDirection = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        rb.AddForce(randomDirection * 20f, ForceMode.Impulse);
    }

    public override void SetDefaults()
    {
        entityName = "Crate";
        entityDescription = "A wooden crate.";

        hasHealth = false;
        isInteractable = false;
        isHoldable = false;
    }
}
