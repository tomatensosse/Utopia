using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[System.Serializable]
public class WorldState
{
    public string saveID;
    public string worldName;
    public int worldSeed;
    public List<Entity> v_activeEntities; // NOT TO BE CONFUSED WITH ENTITYDATAS IN WORLD SAVE

    public WorldSave ToSave()
    {
        WorldSave worldSave = new WorldSave();
        worldSave.saveID = saveID;

        worldSave.worldSaveName = worldName;
        worldSave.worldSeed = worldSeed;

        worldSave.entityDatas = new List<EntityData>();

        foreach (Entity entity in v_activeEntities)
        {
            worldSave.entityDatas.Add(entity.SaveEntity());
        }

        return worldSave;
    }

    public void Load(WorldSave worldSave)
    {
        saveID = worldSave.saveID;
        worldName = worldSave.worldSaveName;
        worldSeed = worldSave.worldSeed;

        v_activeEntities = new List<Entity>();
    }
}

public class World : NetworkBehaviour
{
    public static World Instance { get; private set; }
    public WorldState worldState = new WorldState();

    private void Awake()
    {
        Debug.Log("World Awake");

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There are multiple World instances in the scene.");
            Destroy(gameObject);
        }
    }

    public void SaveWorld()
    {
        WorldSave worldSave = worldState.ToSave();
        Game.Instance.saveSystem.SaveWorld(worldSave, worldSave.saveID);
    }

    public void AddEntity(Entity entity)
    {
        worldState.v_activeEntities.Add(entity);
    }

    public void LoadWorldSave(WorldSave worldSave)
    {
        Debug.Log("Loaded world : " + worldSave.worldSaveName);

        worldState.Load(worldSave);

        foreach (EntityData entityData in worldSave.entityDatas)
        {
            GameObject entity = Instantiate(EntityDatabase.Instance.GetEntityByID(entityData.entityID), entityData.position, entityData.rotation);
            entity.GetComponent<Entity>().LoadEntity(entityData);
            AddEntity(entity.GetComponent<Entity>());

            NetworkServer.Spawn(entity.gameObject, NetworkServer.localConnection);
        }
    }

    public void LoadWorldState(WorldState worldState)
    {
        Debug.Log("Loaded world : " + worldState.worldName);

        foreach (Entity entity in worldState.v_activeEntities)
        {
            GameObject entityGameObject = Instantiate(EntityDatabase.Instance.GetEntityByID(entity.entityID), entity.transform.position, entity.transform.rotation);
            entityGameObject.GetComponent<Entity>().LoadEntity(entity.SaveEntity());

            NetworkServer.Spawn(entityGameObject, NetworkServer.localConnection);
        }
    }
}
