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

    public virtual void Start()
    {
        if (isLoaded) { return; }
        Game.Instance.entities.Add(this);
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
}
