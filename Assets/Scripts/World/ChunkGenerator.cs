using System.Collections;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public static ChunkGenerator Instance { get; private set; }

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
        switch (generationMode)
        {
            case World.GenerationMode.StaticSize:
                // Static world gets generated in start function
                break;
            case World.GenerationMode.TargetPlayers:
                //TargetPlayers();
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

    private void GenerateChunk(Vector3Int position)
    {
        GameObject chunk = new GameObject("Chunk " + position);
        chunk.transform.position = position * World.Settings.chunkSize;
        chunk.transform.SetParent(transform);

        Chunk chunkComponent = chunk.AddComponent<Chunk>();
        chunkComponent.Initialize();

        chunkComponent.biome = biome;

        chunkComponent.Generate();
    }
}