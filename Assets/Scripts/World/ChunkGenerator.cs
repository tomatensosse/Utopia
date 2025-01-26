using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public static ChunkGenerator Instance { get; private set; }

    private List<Chunk> chunks = new List<Chunk>();
    protected List<Chunk> dirtyChunks = new List<Chunk>();

    public Biome biome;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        if (World.WorldGenerationMode == World.GenerationMode.StaticSize)
        {
            StartCoroutine(WaitAndGenerateStatic());
        }
    }

    IEnumerator WaitAndGenerateStatic()
    {
        yield return new WaitUntil(() => MeshGenerator.Ready);
        StaticSize();
    }

    void Update()
    {
        ChooseMode(World.WorldGenerationMode);
    }

    private void ChooseMode(World.GenerationMode generationMode)
    {
        if (!MeshGenerator.Ready || !World.Ready)
        {
            return;
        }

        switch (generationMode)
        {
            case World.GenerationMode.StaticSize:
                // Static world gets generated in start function
                break;
            case World.GenerationMode.TargetTransform:
                TargetTransform();
                break;
            case World.GenerationMode.MultiplayerServerSide:
                //MultiplayerServerSide();
                break;
            case World.GenerationMode.MultiplayerClientUnsafe:
                //MultiplayerClientSide();
                break;
            case World.GenerationMode.MultiplayerClientSafe:
                //MultiplayerClientSide();
                break;
        }
    }

    private void StaticSize()
    {
        Vector3Int worldSize = World.StaticWorldSize;

        for (int x = 0; x < worldSize.x; x++)
        {
            for (int y = 0; y < worldSize.y; y++)
            {
                for (int z = 0; z < worldSize.z; z++)
                {
                    GenerateChunk(new Vector3Int(x, y, z));
                }
            }
        }
    }

    private void TargetTransform()
    {
        foreach (Transform target in World.TargetedTransforms)
        {
            Vector3Int targetPosition = World.WorldToChunkPosition(target.position);

            for (int x = -World.RenderDistanceHorizontal; x < World.RenderDistanceHorizontal; x++)
            {
                for (int y = -World.RenderDistanceVertical; y < World.RenderDistanceVertical; y++)
                {
                    for (int z = -World.RenderDistanceHorizontal; z < World.RenderDistanceHorizontal; z++)
                    {
                        Vector3Int position = targetPosition + new Vector3Int(x, y, z);

                        if (!ChunkExistsAt(position))
                        {
                            GenerateChunk(position);
                        }
                    }
                }
            }
        }

        HandleDirtyChunks();
    }

    private void HandleDirtyChunks()
    {
        foreach (Chunk chunk in chunks)
        {
            if (!ChunkInRenderDistance(chunk.chunkPosition))
            {
                dirtyChunks.Add(chunk);
            }
        }

        while (dirtyChunks.Count > 0)
        {
            Chunk chunk = dirtyChunks[0];
            dirtyChunks.RemoveAt(0);

            chunks.Remove(chunk);
            Destroy(chunk.gameObject);
        }
    }

    private bool ChunkExistsAt(Vector3Int position)
    {
        foreach (Chunk chunk in chunks)
        {
            if (chunk.chunkPosition == position)
            {
                return true;
            }
        }

        return false;
    }

    private bool ChunkInRenderDistance(Vector3Int position)
    {
        foreach (Transform target in World.TargetedTransforms)
        {
            Vector3Int targetPosition = World.WorldToChunkPosition(target.position);

            for (int x = -World.RenderDistanceHorizontal; x < World.RenderDistanceHorizontal; x++)
            {
                for (int y = -World.RenderDistanceVertical; y < World.RenderDistanceVertical; y++)
                {
                    for (int z = -World.RenderDistanceHorizontal; z < World.RenderDistanceHorizontal; z++)
                    {
                        Vector3Int renderPosition = targetPosition + new Vector3Int(x, y, z);

                        if (renderPosition == position)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    private void GenerateChunk(Vector3Int position)
    {
        GameObject chunk = new GameObject("Chunk " + position);
        chunk.transform.position = position * World.Settings.chunkSize;
        chunk.transform.SetParent(transform);

        Chunk chunkComponent = chunk.AddComponent<Chunk>();
        chunkComponent.chunkPosition = position;
        chunkComponent.Initialize();

        chunkComponent.biome = biome;

        chunkComponent.Generate();

        chunks.Add(chunkComponent);
    }
}