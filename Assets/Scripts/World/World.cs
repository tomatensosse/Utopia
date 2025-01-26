using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance { get; private set; }
    public static bool Ready => Instance._isReady;
    public static WorldSettings Settings => Instance._worldSettings;
    public static int Seed => Instance._seed;
    public static GenerationMode WorldGenerationMode => Instance._generationMode;

    private bool _isReady = false;

    public enum GenerationMode
    {
        StaticSize,
        TargetTransform,
        MultiplayerServerSide, // TBA
        MultiplayerClientUnsafe, // TBA
        MultiplayerClientSafe // is it k-dot is it aubrey or me
    }

    public GenerationMode _generationMode;

    #region StaticSize Settings
    public static Vector3Int StaticWorldSize => Instance._staticWorldSize;
    [ShowInInspector, ShowIf("_generationMode", GenerationMode.StaticSize)]
    public Vector3Int _staticWorldSize = new Vector3Int(4, 2, 4);
    #endregion

    #region TargetPlayers Settings
    public static List<Transform> TargetedTransforms => Instance._targetedTransforms;
    [ShowInInspector, ShowIf("_generationMode", GenerationMode.TargetTransform)]
    private List<Transform> _targetedTransforms = new List<Transform>();

    #endregion

    #region MultiplayerServerSide Settings

    #endregion

    #region MultiplayerClientSide Settings


    #endregion
    
    [ShowIf("@_generationMode != GenerationMode.StaticSize")]
    public static int RenderDistanceHorizontal => Instance.renderDistanceHorizontal;
    public static int RenderDistanceVertical => Instance.renderDistanceVertical;
    public int renderDistanceHorizontal = 8, renderDistanceVertical = 8;

    [Header("World Settings")]
    public int setSeed = 0;
    private int _seed;

    public int chunkSize = 16;
    public int numPointsPerAxis = 8;

    public const int threadGroupSize = 8;

    public struct WorldSettings
    {
        public int chunkSize;

        // Values that will be used by generators (MeshGenerator, DensityNode, etc...)

        // Storing them in a struct so they can be accessed from anywhere +
        // multiplayer structure will be MUCH easier to implement

        public int numPoints;
        public int numPointsPerAxis;
        public int numVoxelsPerAxis;
        public int numVoxels;
        public int maxTriangleCount;
        public int numThreadsPerAxis;
        public int boundsSize;
        public float pointSpacing;
        public Vector3 worldBounds; // or worldSize

        public int threadGroupSize;
    }

    private WorldSettings _worldSettings;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        GenerateConstants();
    }

    private void GenerateConstants()
    {
        int numVoxelsPerAxis = numPointsPerAxis - 1;
        int numVoxels = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
        int maxTriangleCount = numVoxels * 5;

        Vector3 worldBounds = new Vector3(renderDistanceHorizontal, renderDistanceVertical, renderDistanceHorizontal) * chunkSize;

        if (_generationMode == GenerationMode.StaticSize)
        {
            worldBounds = new Vector3(StaticWorldSize.x, StaticWorldSize.y, StaticWorldSize.z) * chunkSize;
        }

        _worldSettings = new WorldSettings
        {
            chunkSize = chunkSize,

            // Shared values
            numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis,
            numPointsPerAxis = numPointsPerAxis,
            numVoxelsPerAxis = numVoxelsPerAxis,
            numVoxels = numVoxels,
            maxTriangleCount = maxTriangleCount,
            numThreadsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float) threadGroupSize),
            boundsSize = chunkSize,
            pointSpacing = chunkSize / ((float)numPointsPerAxis - 1),
            worldBounds = worldBounds,

            threadGroupSize = threadGroupSize
        };

        Debug.Log("World is ready!");
        _isReady = true;
    }

    public static void RegisterTransform(Transform target)
    {
        Instance._targetedTransforms.Add(target);
    }

    public static Vector3 ChunkToWorldPosition(Vector3Int chunkPosition)
    {
        return new Vector3(chunkPosition.x * Settings.chunkSize, chunkPosition.y * Settings.chunkSize, chunkPosition.z * Settings.chunkSize);
    }

    public static Vector3Int WorldToChunkPosition(Vector3 worldPosition)
    {
        return new Vector3Int(Mathf.FloorToInt(worldPosition.x / Settings.chunkSize), Mathf.FloorToInt(worldPosition.y / Settings.chunkSize), Mathf.FloorToInt(worldPosition.z / Settings.chunkSize));
    }
}