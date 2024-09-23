using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Mirror;
using Steamworks;
using UnityEngine;

public class World : NetworkBehaviour
{
    public static World Instance { get; private set; }
    public WorldData worldData;
    [HideInInspector] public bool initialized;
    public List<PlayerLocation> playerLocations;

    public float worldTimeSession;
    public float tickRate = 10f;

    [Header("Spawning")]
    public int maxSpawnsPerTick = 3;
    public int minimumSpawnDistance = 1;
    public int maximumSpawnDistance = 6;
    public delegate void SpawnTickCallback();
    public static event TimerCallback OnSpawnTick;

    [Header("World Settings")]
    public int seed;
    [HideInInspector] public int[,] biomeMap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There are multiple World instances in the scene.");
            Destroy(gameObject);
        }

        Random.InitState(seed);

        initialized = true;
    }

    private void Start()
    {
        InitializeWorld();

        InvokeRepeating("Tick", 0f, tickRate);
    }

    private void Tick()
    {
        Debug.Log("Tick");
        OnSpawnTick?.Invoke(null);
    }

    private void Update()
    {
        worldTimeSession = Time.timeSinceLevelLoad;
    }

    private void InitializeWorld()
    {
        biomeMap = MapGenerator.Instance.BiomeMap(seed); // Generate biome map

        MeshGenerator.Instance.Initialize(seed); // Initialize mesh generator
    }

    public void UnloadChunk(ChunkData chunkData) // Save chunk
    {
        worldData.chunkDatas.Add(chunkData);
    }

    public ChunkData LoadChunk(Vector2Int at) // Load from save
    {
        foreach (ChunkData chunkData in worldData.chunkDatas)
        {
            if (chunkData.chunkPosition == at)
            {
                Debug.Log("NIGGANIGGA | Chunk at " + at + " found in save.");

                return chunkData;
            }
        }

        Debug.Log("Chunk at " + at + " not found in save.");

        return null;
    }

    public void AppendPlayer(Transform player)
    {
        if (playerLocations == null)
        {
            playerLocations = new List<PlayerLocation>();
        }

        PlayerLocation playerLocation = new PlayerLocation();
        playerLocation.player = player;
        playerLocations.Add(playerLocation);
    }
}
