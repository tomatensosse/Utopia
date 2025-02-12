using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public GameObject lootBoxPrefab;

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnLootBox();
        }
    }

    [Server]
    public void SpawnLootBox()
    {
        Transform spawnPoint = SpawnPointManager.Instance.GetRandomLootSpawnPoint();

        GameObject lootBox = Instantiate(lootBoxPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(lootBox);
    }
}