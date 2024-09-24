using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public static MeshGenerator Instance { get; private set; }
    public MapGenerator generator;
    private bool initialized = false;

    [Header("World Generation")]
    float newNoise;
    public Material mat;
    public Biome defaultBiome;

    [Header("Generation")]
    public int chunkSizeHorizontal = 8;
    public int chunkSizeVertical = 128;
    public int numPointsPerAxis = 8;
    public bool fixedSize = false;
    [ConditionalHide(nameof(fixedSize), true)]
    int offset;

    [ConditionalHide(nameof(fixedSize), false)]
    public int renderDistance = 3;
    [ConditionalHide(nameof(fixedSize), false)]
    public int simulationDistance = 3;

    public int biomeEdgeBlending = 2;

    public bool generateColliders = false;

    GameObject chunksHolder;
    string chunksHolderName = "Chunks Holder";

    List<Chunk> chunks = new List<Chunk>();
    List<GameObject> chunksGameObjects = new List<GameObject>();
    List<GameObject> chunksToDestroy = new List<GameObject>();
    public TMP_Text playerChunkPositionText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There are multiple MeshGenerator instances in the scene.");
            Destroy(gameObject);
        }

        DestroyAllChunks();
    }

    private void Update()
    {
        if (!initialized)
        {
            return;
        }

        if (World.Instance.playerLocations == null)
        {
            return;
        }

        foreach (PlayerLocation playerLocation in World.Instance.playerLocations)
        {
            Vector2Int playerInChunk = playerLocation.chunkPosition;
            GetPlayerCurrentPosition(playerLocation);

            if (playerInChunk == playerLocation.chunkPosition)
            {
                return; // Dont generate anything if the player is in the same chunk
            }

            Generate(playerLocation.chunkPosition);
        }
    }

    public void Initialize(int seed)
    {
        if (chunksHolder == null) 
        {
            if (GameObject.Find (chunksHolderName)) {
                chunksHolder = GameObject.Find (chunksHolderName);
            } else {
                chunksHolder = new GameObject (chunksHolderName);
                chunksHolder.AddComponent<ChunksHolder>();
            }
        }

        offset = chunkSizeHorizontal * generator.mapSizeInChunks / 2;

        Random.InitState(seed);

        newNoise = seed;

        initialized = true;
    }

    void Generate(Vector2Int playerInChunk)
    {
        GetOutOfRangeChunks(playerInChunk);
        GenerateNewChunks(playerInChunk);
    }

    public List<Vector3> SumVertices(List<Vector3> vx, List<Vector3> vy)
    {
        List<Vector3> sum = new List<Vector3>();

        int longer = 0;

        if (vx.Count > vy.Count) { longer = vx.Count; }
        if (vy.Count > vx.Count) { longer = vy.Count; }
        if (vx.Count == vy.Count) { longer = vx.Count; }

        if (vx.Count == 0)
        {
            vx = vy;
            return vx;
        }

        for (int i = 0; i < longer; i++)
        {
            sum.Add(new Vector3(vx[i].x, vx[i].y + + vy[i].y, vx[i].z));
        }

        return sum;
    }

    public List<int> SumTriangles(List<int> vx, List<int> vy)
    {
        List <int> sum = new List<int>();

        int longer = 0;

        if (vx.Count > vy.Count) { longer = vx.Count; }
        if (vy.Count > vx.Count) { longer = vy.Count; }
        if (vx.Count == vy.Count) { longer = vx.Count; }

        if (vx.Count == 0)
        {
            vx = vy;
            return vx;
        }

        for (int i = 0; i < longer; i++)
        {
            sum.Add(vx[i] + vy[i]);
        }

        return sum;
    }

    public MeshData GenerateMesh(Biome currentChunkBiome, Vector2 currentChunkPosition, GameObject currentChunk)
    {
        Chunk boraChunk = currentChunk.GetComponent<Chunk>();

        List<MeshData> meshDatas = new List<MeshData>();

        RelativeBiomes relativeBiomes = new RelativeBiomes();
        relativeBiomes.Feed(boraChunk.chunkPosition, World.Instance.biomeMap);

        foreach (GameObject heightGenGameObject in defaultBiome.heightGenerators)
        {
            TerrainHeightGen terrainHeightGen = heightGenGameObject.GetComponent<TerrainHeightGen>();
            meshDatas.Add(terrainHeightGen.Execute(chunkSizeHorizontal, numPointsPerAxis, currentChunkPosition, newNoise));
        }

        if (currentChunkBiome != defaultBiome)
        {
            foreach (GameObject heightGenGameObject in currentChunkBiome.heightGenerators)
            {
                TerrainHeightGen terrainHeightGen = heightGenGameObject.GetComponent<TerrainHeightGen>();
                MeshData meshDataRaw = terrainHeightGen.Execute(chunkSizeHorizontal, numPointsPerAxis, currentChunkPosition, newNoise);
                MeshData meshDataProcessed = new MeshData();

                Vector3[] verticesRaw = meshDataRaw.vertices.ToArray();
                Vector3[] verticesProcessed = verticesRaw;

                float step = 1f / biomeEdgeBlending;

                if (relativeBiomes.right)
                {
                    boraChunk.rightEdgeInterpolated = true;

                    for (int z = 0; z < (chunkSizeHorizontal + 1); z++)
                    {
                        for (int x = 0; x < biomeEdgeBlending; x++)
                        {
                            verticesProcessed[((chunkSizeHorizontal + 1) + z) + (((x - 1) * (chunkSizeHorizontal + 1)))].y = verticesRaw[((chunkSizeHorizontal + 1) + z) + (((x - 1) * (chunkSizeHorizontal + 1)))].y * (step * x);
                        }
                    }
                }

                if (relativeBiomes.left)
                {
                    boraChunk.leftEdgeInterpolated = true;

                    for (int z = 0; z < (chunkSizeHorizontal + 1); z++)
                    {
                        for (int x = 0; x < biomeEdgeBlending; x++)
                        {
                            verticesProcessed[((chunkSizeHorizontal + 1) * ((chunkSizeHorizontal - x)) + z)].y = verticesRaw[((chunkSizeHorizontal + 1) * ((chunkSizeHorizontal - x)) + z)].y * (step * x);
                        }
                    }
                }

                if (relativeBiomes.front)
                {
                    boraChunk.frontEdgeInterpolated = true;

                    for (int x = 0; x < (chunkSizeHorizontal + 1); x++)
                    {
                        for (int z = 0; z < biomeEdgeBlending; z++)
                        {
                            verticesProcessed[(x * (chunkSizeHorizontal + 1)) + z].y = verticesRaw[(x * (chunkSizeHorizontal + 1)) + z].y * (step * z);
                        }
                    }
                }

                if (relativeBiomes.back)
                {
                    boraChunk.backEdgeInterpolated = true;

                    for (int x = 0; x < (chunkSizeHorizontal + 1); x++)
                    {
                        for (int z = 0; z < biomeEdgeBlending; z++)
                        {
                            verticesProcessed[(x * (chunkSizeHorizontal + 1)) + (chunkSizeHorizontal - z)].y = verticesRaw[(x * (chunkSizeHorizontal + 1)) + (chunkSizeHorizontal - z)].y * (step * z);
                        }
                    }
                }

                if (relativeBiomes.frontRight && !boraChunk.frontEdgeInterpolated && !boraChunk.rightEdgeInterpolated)
                {
                    boraChunk.frontRightEdgeInterpolated = true;

                    for (int x = 0; x < biomeEdgeBlending; x++)
                    {
                        for (int z = 0; z < biomeEdgeBlending; z++)
                        {
                            IntBool intBool = IntBool.LowerInt(z, x);
                            if (intBool.sBool)
                            {
                                //Debug.Log(verticesProcessed[((chunkSizeHorizontal + 1) + z) + (((x - 1) * (chunkSizeHorizontal + 1)))] + " with div: " + (step * intBool.i));
                                verticesProcessed[((chunkSizeHorizontal + 1) + z) + (((x - 1) * (chunkSizeHorizontal + 1)))].y = verticesRaw[((chunkSizeHorizontal + 1) + z) + (((x - 1) * (chunkSizeHorizontal + 1)))].y * (step * x);
                            }
                            if (!intBool.sBool)
                            {
                                //Debug.Log(verticesProcessed[(x * (chunkSizeHorizontal + 1)) + z] + " with div: " + (step * intBool.i));
                                verticesProcessed[(x * (chunkSizeHorizontal + 1)) + z].y = verticesRaw[(x * (chunkSizeHorizontal + 1)) + z].y * (step * z);
                            }
                        }
                    }
                }

                if (relativeBiomes.frontLeft && !boraChunk.frontEdgeInterpolated && !boraChunk.leftEdgeInterpolated)
                {
                    boraChunk.frontLeftEdgeInterpolated = true;

                    for (int x = 0; x < biomeEdgeBlending; x++)
                    {
                        for (int z = 0; z < biomeEdgeBlending; z++)
                        {
                            IntBool intBool = IntBool.LowerInt(z, x);
                            if (intBool.sBool)
                            {
                                //Debug.Log(verticesProcessed[((chunkSizeHorizontal - x) * (chunkSizeHorizontal + 1)) + z] + " - - - a");
                                verticesProcessed[((chunkSizeHorizontal - x) * (chunkSizeHorizontal + 1)) + z].y = verticesRaw[((chunkSizeHorizontal - x) * (chunkSizeHorizontal + 1)) + z].y * (step * x);
                            }
                            if (!intBool.sBool)
                            {
                                //Debug.Log(verticesProcessed[((chunkSizeHorizontal + 1) * ((chunkSizeHorizontal - x)) + z)] + " - - - b");
                                verticesProcessed[((chunkSizeHorizontal + 1) * ((chunkSizeHorizontal - x)) + z)].y = verticesRaw[((chunkSizeHorizontal + 1) * ((chunkSizeHorizontal - x)) + z)].y * (step * z);
                            }
                        }
                    }
                }

                if (relativeBiomes.backRight && !boraChunk.backEdgeInterpolated && !boraChunk.rightEdgeInterpolated)
                {
                    boraChunk.backRightEdgeInterpolated = true;

                    for (int x = 0; x < biomeEdgeBlending; x++)
                    {
                        for (int z = 0; z < biomeEdgeBlending; z++)
                        {
                            IntBool intBool = IntBool.LowerInt(z, x);
                            if (intBool.sBool)
                            {
                                //Debug.Log(verticesProcessed[((chunkSizeHorizontal + 1) + (chunkSizeHorizontal - z)) + (((x - 1) * (chunkSizeHorizontal + 1)))] + " - aaa" + "x: " + x + " z: " + z);
                                verticesProcessed[((chunkSizeHorizontal + 1) + (chunkSizeHorizontal - z)) + (((x - 1) * (chunkSizeHorizontal + 1)))].y = verticesRaw[((chunkSizeHorizontal + 1) + (chunkSizeHorizontal - z)) + (((x - 1) * (chunkSizeHorizontal + 1)))].y * (step * x);
                            }
                            if (!intBool.sBool)
                            {
                                //Debug.Log(verticesProcessed[(x * (chunkSizeHorizontal + 1)) + (chunkSizeHorizontal - z)] + " - bbb");
                                verticesProcessed[(x * (chunkSizeHorizontal + 1)) + (chunkSizeHorizontal - z)].y = verticesRaw[(x * (chunkSizeHorizontal + 1)) + (chunkSizeHorizontal - z)].y * (step * z);
                            }
                        }
                    }
                }

                if (relativeBiomes.backLeft && !boraChunk.backEdgeInterpolated && !boraChunk.leftEdgeInterpolated)
                {
                    boraChunk.backLeftEdgeInterpolated = true;

                    for (int x = 0; x < biomeEdgeBlending; x++)
                    {
                        for (int z = 0; z < biomeEdgeBlending; z++)
                        {
                            IntBool intBool = IntBool.LowerInt(z, x);
                            if (intBool.sBool)
                            {
                                //Debug.Log(verticesProcessed[(chunkSizeHorizontal - z) + ((chunkSizeHorizontal - x) * (chunkSizeHorizontal + 1))] + " - aaa" + "x: " + x + " z: " + z);
                                verticesProcessed[(chunkSizeHorizontal - z) + ((chunkSizeHorizontal - x) * (chunkSizeHorizontal + 1))].y = verticesRaw[(chunkSizeHorizontal - z) + ((chunkSizeHorizontal - x) * (chunkSizeHorizontal + 1))].y * (step * x);
                            }
                            if (!intBool.sBool)
                            {
                                //Debug.Log(verticesProcessed[(chunkSizeHorizontal + 1) * (chunkSizeHorizontal - x) + (chunkSizeHorizontal - z)] + " - bbb");
                                verticesProcessed[(chunkSizeHorizontal + 1) * (chunkSizeHorizontal - x) + (chunkSizeHorizontal - z)].y = verticesRaw[(chunkSizeHorizontal + 1) * (chunkSizeHorizontal - x) + (chunkSizeHorizontal - z)].y * (step * z);
                            }
                        }
                    }
                }

                meshDataProcessed.vertices = verticesProcessed.ToList();
                meshDataProcessed.triangles = meshDataRaw.triangles;
                meshDatas.Add(meshDataProcessed);
            }

            if (currentChunkBiome.heightGenerators.Length == 0)
            {
                Debug.LogWarning("Warning, " + currentChunkBiome + " does not have any heightGenerator assigned to it. ");
            }
        }

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        
        foreach (MeshData meshData in meshDatas)
        {
            vertices = SumVertices(vertices, meshData.vertices);
            //triangles = sumTwoListsInt(triangles, meshData.triangles.ToList());
        }

        MeshData meshDataFinal = new MeshData();
        meshDataFinal.vertices = vertices;
        meshDataFinal.triangles = meshDatas[0].triangles;

        return meshDataFinal;
    }

    void GenerateNewChunks(Vector2Int playerInChunk)
    {
        while (chunksToDestroy.Count > 0)
        {
            GameObject chunkToDestroy = chunksToDestroy[0];
            Chunk chunk = chunkToDestroy.GetComponent<Chunk>();

            chunksToDestroy.Remove(chunkToDestroy);
            chunks.Remove(chunkToDestroy.GetComponent<Chunk>());
            chunksGameObjects.Remove(chunkToDestroy);

            ChunkData chunkData = chunk.GetChunkData();
            World.Instance.UnloadChunk(chunkData);

            Destroy(chunkToDestroy);
        }

        chunksToDestroy.Clear();

        bool newChunksGenerated = false;

        for (int r = -renderDistance; r <= renderDistance; r++)
        {
            for (int c = -renderDistance; c <= renderDistance; c++)
            {
                Vector2Int chunkPosition = new Vector2Int(r + playerInChunk.x, c + playerInChunk.y);

                bool chunkExists = false;

                foreach (Chunk chunk in chunks)
                {
                    if (chunk.chunkPosition == chunkPosition)
                    {   
                        chunkExists = true;
                    }
                }

                if (!chunkExists)
                {
                    GameObject chunkGameObject = new GameObject("Chunk " + chunkPosition);
                    chunkGameObject.transform.parent = chunksHolder.transform;
                    chunkGameObject.transform.position = new Vector3(chunkPosition.x * chunkSizeHorizontal - offset, 0, chunkPosition.y * chunkSizeHorizontal - offset);

                    Chunk boraChunk = chunkGameObject.AddComponent<Chunk>();
                    boraChunk.chunkPosition = chunkPosition;

                    boraChunk.chunkSizeHorizontal = chunkSizeHorizontal;
                    boraChunk.chunkSizeVertical = chunkSizeVertical;

                    Biome biome = generator.GetBiome(chunkPosition);
                    boraChunk.chunkBiome = biome;
                    boraChunk.chunkBoundsColor = biome.biomeColor;

                    chunks.Add(boraChunk);
                    chunksGameObjects.Add(chunkGameObject);

                    ChunkData chunkData = World.Instance.LoadChunk(chunkPosition);

                    newChunksGenerated = true;

                    if (chunkData != null)
                    {
                        boraChunk.SetMesh(chunkData.vertices, chunkData.triangles);
                        boraChunk.Configure(mat);

                        Debug.Log("Loading chunk at " + chunkPosition);
            
                        LoadSpawnables(boraChunk, chunkData.spawnableDatas);
                        LoadEntities(boraChunk, chunkData.entityDatas);
                    }

                    if (!boraChunk.setUp)
                    {
                        MeshData meshData;
                        meshData = GenerateMesh(boraChunk.chunkBiome, new Vector2(chunkGameObject.transform.position.x, chunkGameObject.transform.position.z), chunkGameObject);

                        if (generateColliders)
                        {
                            boraChunk.generateColliders = true;
                        }
                            
                        boraChunk.SetMesh(meshData.vertices, meshData.triangles);

                        boraChunk.Configure(mat);
                    }

                    if (!boraChunk.spawnablesHaveBeenGenerated)
                    {
                        GenerateSpawnables(boraChunk, biome);
                    }
                }
            }
        }

        if (newChunksGenerated)
        {
            Debug.Log("New chunks generated");

            foreach (Chunk chunk in chunks)
            {
                chunk.CheckIfInSimulationDistance();
            }

            ChunksHolder.Instance.BuildNavMesh();
        }
    }

    void GetPlayerCurrentPosition(PlayerLocation playerLocation)
    {
        Vector3 playerPosition = playerLocation.player.position;
        int offset = chunkSizeHorizontal * generator.mapSizeInChunks / 2;
        Vector2Int playerInChunk = new Vector2Int(
            (int)(playerPosition.x + offset) / chunkSizeHorizontal, 
            (int)(playerPosition.z + offset) / chunkSizeHorizontal
        );

        playerLocation.chunkPosition = playerInChunk;
        playerLocation.playerPosition = playerPosition;
    }

    public Vector2Int InChunk(Vector3 position)
    {
        int offset = chunkSizeHorizontal * generator.mapSizeInChunks / 2;
        Vector2Int inChunk = new Vector2Int(
            (int)(position.x + offset) / chunkSizeHorizontal, 
            (int)(position.z + offset) / chunkSizeHorizontal
        );

        return inChunk;
    }

    void DestroyAllChunks()
    {
        chunks.Clear();
        chunksGameObjects.Clear();

        if (chunksHolder != null)
        {
            DestroyImmediate(chunksHolder);
            chunksHolder = new GameObject(chunksHolderName);
        }
    }

    void GetOutOfRangeChunks(Vector2Int playerInChunk)
    {
        foreach (GameObject chunkGameObject in chunksGameObjects)
        {
            Chunk boraChunk = chunkGameObject.GetComponent<Chunk>();

            if (boraChunk.chunkPosition.x < playerInChunk.x - renderDistance || boraChunk.chunkPosition.x > playerInChunk.x + renderDistance || boraChunk.chunkPosition.y < playerInChunk.y - renderDistance || boraChunk.chunkPosition.y > playerInChunk.y + renderDistance)
            {
                chunksToDestroy.Add(chunkGameObject);
            }
        }
    }

    void LoadSpawnables(Chunk chunk, List<SpawnableData> spawnableDatas)
    {
        if (spawnableDatas == null)
        {
            return;
        }

        foreach (SpawnableData spawnableData in spawnableDatas)
        {
            GameObject spawnableGameObject = Instantiate(SpawnableDatabase.Instance.GetSpawnableByID(spawnableData.spawnableID));

            Debug.Log(spawnableData.spawnableID);

            spawnableGameObject.transform.SetParent(chunk.gameObject.transform);
            spawnableGameObject.transform.localPosition = spawnableData.localPosition;
            spawnableGameObject.transform.localRotation = Quaternion.Euler(spawnableData.localRotation);
            spawnableGameObject.transform.localScale = spawnableData.localScale;

            chunk.spawnablesInChunk.Add(spawnableGameObject);

            NetworkServer.Spawn(spawnableGameObject);
        }

        chunk.spawnablesHaveBeenGenerated = true;
    }

    void LoadEntities(Chunk chunk, List<EntityData> entityDatas)
    {
        if (entityDatas == null)
        {
            return;
        }

        foreach (EntityData entityData in entityDatas)
        {
            GameObject entityGameObject = Instantiate(EntityDatabase.Instance.GetEntityByID(entityData.entityID));

            entityGameObject.transform.localPosition = entityData.position;
            entityGameObject.transform.localRotation = entityData.rotation;

            NetworkServer.Spawn(entityGameObject);
        }
    }

    void GenerateSpawnables(Chunk chunk, Biome biome)
    {
        if (chunk.spawnablesHaveBeenGenerated)
        {
            return;
        }

        foreach (GameObject spawnableGameObject in biome.spawnables)
        {
            Mesh mesh = chunk.GetMesh();
            Vector3 spawnPosition = mesh.vertices[Random.Range(0, mesh.vertices.Length)];

            Spawnable spawnableComponent = spawnableGameObject.GetComponent<Spawnable>();

            if (spawnableComponent.generateChance >= Random.Range(0f, 1f))
            {
                GameObject spawnedObject = Instantiate(spawnableGameObject);

                spawnedObject.transform.SetParent(chunk.gameObject.transform);
                spawnedObject.transform.localPosition = spawnPosition;

                chunk.spawnablesInChunk.Add(spawnedObject);

                NetworkServer.Spawn(spawnedObject);
            }
        }

        chunk.spawnablesHaveBeenGenerated = true;
    }

    public Chunk GetChunk(Vector2Int at)
    {
        foreach (Chunk chunk in chunks)
        {
            if (chunk.chunkPosition == at)
            {
                return chunk;
            }
        }

        return null;
    }
}