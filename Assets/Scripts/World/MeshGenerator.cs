#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEditor;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public List<Transform> players;
    private List<PlayerLocation> playerLocations;
    public MapGenerator generator;

    [Header("World Generation")]
    public bool setSeed = false;
    public int seed;
    float newNoise;
    public Material mat;
    public Biome defaultBiome;

    [Header("Generation")]
    public int chunkSizeHorizontal = 8;
    public int chunkSizeVertical = 128;
    public int numPointsPerAxis = 8;
    public bool fixedSize = false;
    [ConditionalHide(nameof(fixedSize), true)]
    public Vector2Int worldSize;
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

    [Header("World Visualisation as PNG")]

    public bool generateImage = false;
    [ConditionalHide(nameof(generateImage), true)]
    public string imageSavePath = "Assets/World.png";
    [ConditionalHide(nameof(generateImage), true)]
    [SerializeField] UnityEngine.UI.Image worldVisualisationUIImage;

    [Header("Debug Attributes")]
    public bool debug = false;
    [ConditionalHide(nameof(debug), true)]
    public bool debugPrintRelativeBiomes = false;
    [ConditionalHide(nameof(debug), true)]
    public bool debugPrintArrayAddition = false;
    [ConditionalHide(nameof(debug), true)]
    public bool debugPrintBiomeInterpolation = false;
    [ConditionalHide(nameof(debug), true)]
    public bool debugPrintBiomeInterpolationEdges = false;
    [ConditionalHide(nameof(debug), true)]
    public TMP_Text playerChunkPositionText;

    void Awake()
    {
        if (!debug)
        {
            playerChunkPositionText.gameObject.SetActive(false);
        }

        if (!setSeed) { seed = UnityEngine.Random.Range(0, 1000000);}
        
        UnityEngine.Random.InitState(seed);

        newNoise = UnityEngine.Random.Range(0, 1000000);

        generator.GenerateMap(seed);

        if (generateImage)
        {
            Debug.LogWarning("Warning. Local world visualisation is currently set to true. This may cause a lot of lag!");
        }

        DestroyAllChunks();
    }

    private void Update()
    {
        foreach (PlayerLocation playerLocation in playerLocations)
        {
            Vector2Int playerInChunk = playerLocation.chunkPosition;
            GetPlayerCurrentPosition(playerLocation);

            if (playerInChunk == playerLocation.chunkPosition)
            {
                return; // Dont generate anything if the player is in the same chunk
            }

            GenerateWorld(playerLocation);
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (chunksHolder == null) 
        {
            if (GameObject.Find (chunksHolderName)) {
                chunksHolder = GameObject.Find (chunksHolderName);
            } else {
                chunksHolder = new GameObject (chunksHolderName);
            }
        }

        foreach (Transform player in players)
        {
            PlayerLocation playerLocation = new PlayerLocation();
            playerLocations.Add(playerLocation);
            GetPlayerCurrentPosition(playerLocation);
        }

        offset = chunkSizeHorizontal * generator.mapSizeInChunks / 2;
    }

    void GenerateWorld(PlayerLocation playerLocation)
    {
        /*
        if (GenerateNewChunks(playerLocation.chunkPosition))
        {
            GetOutOfRangeChunks(playerLocation.chunkPosition);

            GenerateMeshes();

            SpawnGenerateables();

            // Sync on network 
        }
        */
    }

    public List<Vector3> sumTwoListsVector(List<Vector3> vx, List<Vector3> vy)
    {
        if (vx.Count != vy.Count && debugPrintArrayAddition)
        {
            Debug.LogWarning("Vector lists are not equal in size");
            Debug.LogWarning(vx.Count + " -VECTOR3- "+  vy.Count);
        }

        List<Vector3> sum = new List<Vector3>();

        int longer = 0;

        if (vx.Count > vy.Count) { longer = vx.Count; }
        if (vy.Count > vx.Count) { longer = vy.Count; }
        if (vx.Count == vy.Count) { longer = vx.Count; }

        if (vx.Count == 0)
        {
            if (debugPrintArrayAddition) { Debug.Log("vx is empty, setting vx to vy"); }
            vx = vy;
            return vx;
        }

        for (int i = 0; i < longer; i++)
        {
            sum.Add(new Vector3(vx[i].x, vx[i].y + + vy[i].y, vx[i].z));
        }

        return sum;
    }

    public List<int> sumTwoListsInt(List<int> vx, List<int> vy)
    {
        if (vx.Count != vy.Count && debugPrintArrayAddition)
        {
            Debug.LogWarning("Int arrays are not equal in size");
            Debug.LogWarning(vx.Count + " -INT- " + vy.Count);
        }

        List <int> sum = new List<int>();

        int longer = 0;

        if (vx.Count > vy.Count) { longer = vx.Count; }
        if (vy.Count > vx.Count) { longer = vy.Count; }
        if (vx.Count == vy.Count) { longer = vx.Count; }

        if (vx.Count == 0)
        {
            if (debugPrintArrayAddition) { Debug.Log("vx is empty, setting vx to vy"); }
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
        relativeBiomes.Feed(boraChunk.chunkPosition, generator);
        relativeBiomes.DebugPrint(currentChunk, debugPrintRelativeBiomes);

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
                    if (debugPrintBiomeInterpolation) { Debug.Log("RIGHT SIDE INTERPOLATED !"); }
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
                    if (debugPrintBiomeInterpolation) { Debug.Log("LEFT SIDE INTERPOLATED !"); }
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
                    if (debugPrintBiomeInterpolation) { Debug.Log("FRONT SIDE INTERPOLATED !"); }
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
                    if (debugPrintBiomeInterpolation) { Debug.Log("BACK SIDE INTERPOLATED !"); }
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
                    if (debugPrintBiomeInterpolationEdges) { Debug.Log("FRONT RIGHT SIDE INTERPOLATED !"); }
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
                    if (debugPrintBiomeInterpolationEdges) { Debug.Log("FRONT LEFT SIDE INTERPOLATED !"); }
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
                    if (debugPrintBiomeInterpolationEdges) { Debug.Log("BACK RIGHT SIDE INTERPOLATED !"); }
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
                    if (debugPrintBiomeInterpolationEdges) { Debug.Log("BACK LEFT SIDE INTERPOLATED !"); }
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
            vertices = sumTwoListsVector(vertices, meshData.vertices);
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

            chunksToDestroy.Remove(chunkToDestroy);
            chunks.Remove(chunkToDestroy.GetComponent<Chunk>());
            chunksGameObjects.Remove(chunkToDestroy);

            Destroy(chunkToDestroy);
        }

        chunksToDestroy.Clear();

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
                    chunkGameObject.transform.position = new Vector3(chunkPosition.x * chunkSizeHorizontal - offset, -16, chunkPosition.y * chunkSizeHorizontal - offset);

                    Chunk boraChunk = chunkGameObject.AddComponent<Chunk>();
                    boraChunk.chunkPosition = chunkPosition;

                    boraChunk.chunkSizeHorizontal = chunkSizeHorizontal;
                    boraChunk.chunkSizeVertical = chunkSizeVertical;

                    Biome biome = generator.GetBiome(chunkPosition);
                    boraChunk.chunkBiome = biome;
                    boraChunk.chunkBoundsColor = biome.biomeColor;

                    chunks.Add(boraChunk);
                    chunksGameObjects.Add(chunkGameObject);
                }
            }
        }
    }

    void GenerateMeshes()
    {
        foreach (GameObject chunkGameObject in chunksGameObjects)
        {
            Chunk boraChunk = chunkGameObject.GetComponent<Chunk>();

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

    void SpawnGenerateables()
    {
        foreach (GameObject chunkGameObject in chunksGameObjects)
        {
            Chunk chunk = chunkGameObject.GetComponent<Chunk>();

            if (!chunk.generateablesHaveBeenGenerated)
            {
                Biome biome = chunk.chunkBiome;
                GenerateSpawnables(chunk, biome);
            }
        }
    }

    void GenerateSpawnables(Chunk chunk, Biome biome)
    {
        foreach (Spawnable spawnable in biome.spawnables)
        {
            Mesh mesh = chunk.GetMesh();
            Vector3 spawnPosition = mesh.vertices[Random.Range(0, mesh.vertices.Length)];

            if (spawnable.generateChance >= UnityEngine.Random.Range(0f, 1f))
            {
                GameObject gameObject = spawnable.GetGameObject();
                GameObject spawnedObject = Instantiate(gameObject);

                spawnedObject.transform.SetParent(chunk.gameObject.transform);
                spawnedObject.transform.localPosition = spawnPosition;

                SpawnableData spawnableData = new SpawnableData();

                spawnableData.position = spawnedObject.transform.position;
                spawnableData.rotation = spawnedObject.transform.rotation.eulerAngles;
                spawnableData.scale = spawnedObject.transform.localScale;
            }
        }

        chunk.generateablesHaveBeenGenerated = true;
    }

    private class PlayerLocation
    {
        public Transform player;
        public Vector2Int chunkPosition;
        public Vector3 playerPosition;
    }
}

#endif