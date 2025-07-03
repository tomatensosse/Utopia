using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public static ChunkGenerator Instance { get; private set; }
    public static Dictionary<Vector3Int, Chunk> Chunks => Instance.chunks;

    private Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
    protected Dictionary<Vector3Int, Chunk> dirtyChunks = new Dictionary<Vector3Int, Chunk>();

    private int staticSizeState = -1;
    private bool isBusy = false;

    [Header("Debug")]
    public bool showDensities = false;

    public delegate void BlendFinished();
    public BlendFinished OnBlendFinished;

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
        if (World.WorldGenerationMode == World.GenerationMode.WorldEditor)
        {
            Debug.LogWarning("WorldEditor ACTIVE.");
        }

        if (World.WorldGenerationMode == World.GenerationMode.StaticSize)
        {
            StartCoroutine(WaitAndGenerateStatic());
        }
    }

    IEnumerator WaitAndGenerateStatic()
    {
        yield return new WaitUntil(() => MeshGenerator.Ready);
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
            case World.GenerationMode.WorldEditor:
                // WorldEditor();
                break;
            case World.GenerationMode.StaticSize:
                StaticSizeState();
                break;
            case World.GenerationMode.TargetTransform:
                TargetTransform();
                break;
        }
    }

    private void StaticSizeState()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isBusy)
        {
            switch (staticSizeState)
            {
                case -1:
                    isBusy = false;
                    Debug.Log($"Idle state | Press space to advance state | Seed: {World.Seed}");
                    break;
                case 0:
                    isBusy = true;
                    Debug.Log("State 0: Generate Chunks with Biomes assigned from MegaBiome.");
                    StaticSize();
                    isBusy = false;
                    Debug.Log("Done!");
                    break;
                case 1:
                    isBusy = true;
                    Debug.Log("State 1: Generate Densities for each chunk.");
                    foreach (Chunk chunk in chunks.Values)
                    {
                        chunk.GenerateDensity();
                    }
                    isBusy = false;
                    Debug.Log("Done!");
                    break;
                case 2:
                    isBusy = true;
                    Debug.Log("State 3: Generate mesh for each chunk.");
                    foreach (Chunk chunk in chunks.Values)
                    {
                        GenerateChunkMesh(chunk);
                    }
                    isBusy = false;
                    Debug.Log("Done!");
                    break;
                case 4:
                    Debug.Log("State 4: Done!");
                    break;
                case 5:
                    isBusy = true;
                    Debug.Log("Cleaning up...");
                    CleanUp();
                    isBusy = false;
                    Debug.Log("Done!");
                    staticSizeState = -1;
                    break;
            }
            
            staticSizeState++;
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
        foreach (Chunk chunk in chunks.Values)
        {
            if (!ChunkInRenderDistance(chunk.chunkPosition))
            {
                dirtyChunks.Add(chunk.chunkPosition, chunk);
            }
        }

        while (dirtyChunks.Count > 0)
        {
            KeyValuePair<Vector3Int, Chunk> chunk = dirtyChunks.First();
            dirtyChunks.Remove(chunk.Key);

            if (!ChunkInRenderDistance(chunk.Key))
            {
                chunks.Remove(chunk.Key);
                Destroy(chunk.Value.gameObject);
            }
        }
    }

    private bool ChunkExistsAt(Vector3Int position)
    {
        if (chunks.ContainsKey(position))
        {
            return true;
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

    public void GenerateChunk(Vector3Int position)
    {
        GameObject chunk = new GameObject("Chunk " + position);
        chunk.transform.position = position * World.Settings.chunkSize;
        chunk.transform.SetParent(transform);

        Chunk chunkComponent;

        if (World.WorldGenerationMode == World.GenerationMode.WorldEditor)
        {
            chunkComponent = chunk.AddComponent<EditorChunk>();
        }
        else
        {
            chunkComponent = chunk.AddComponent<Chunk>();
        }

        chunkComponent.biome = World.Instance.SampleBiomeForChunk(position);

        chunkComponent.chunkPosition = position;
        chunkComponent.Initialize();

        chunks.Add(position, chunkComponent);
    }

    public void GenerateDensities()
    {
        foreach (var kvp in chunks)
        {
            if (kvp.Value.biome == null)
            {
                Debug.LogError($"{kvp.Value.chunkPosition} | Chunk has no biome");
                continue;
            }

            kvp.Value.GenerateDensity();
        }
    }

    public void RenderDensities()
    {
        if (!showDensities)
        {
            return;
        }

        foreach (var kvp in chunks)
        {
            if (!kvp.Value.isDensityGenerated)
            {
                Debug.LogError($"{kvp.Value.chunkPosition} | Densities can't be visualised");
                continue;
            }

            if (kvp.Value.TryGetComponent<EditorChunk>(out EditorChunk editorChunk))
            {
                editorChunk.RenderDensity();
            }
        }
    }

    public void GenerateMeshes()
    {
        foreach (var kvp in chunks)
        {
            if (!kvp.Value.isDensityGenerated)
            {
                Debug.LogError($"{kvp.Value.chunkPosition} | Mesh can't be generated");
                continue;
            }

            GenerateChunkMesh(kvp.Value);
        }
    }

    private void GenerateChunkMesh(Chunk chunk)
    {
        chunk.GenerateMesh();
    }

    private void CleanUp() // DANGEROUS METHOD
    {
        Debug.LogWarning("CleanUp Method Called | This shouldn't be called unless you are debugging...");

        var chunksToRemove = chunks.Values.ToList();

        foreach (var chunk in chunksToRemove)
        {
            chunks.Remove(chunk.chunkPosition);
            Destroy(chunk.gameObject);
        }
    }
}