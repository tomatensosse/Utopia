using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Object : Entity
{

    public override void Start()
    {
        base.Start();

        if (wasLoadedBefore) { return; }

        if (entityRigidbody == null)
        {
            entityRigidbody = gameObject.AddComponent<Rigidbody>();
        }

        Vector3 randomDirection = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        entityRigidbody.AddForce(randomDirection * 20f, ForceMode.Impulse);
    }
}
