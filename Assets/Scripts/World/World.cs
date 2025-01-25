using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance { get; private set; }
    public static WorldSettings Settings => Instance._worldSettings;
    public static int Seed => Instance._seed;
    public static GenerationMode WorldGenerationMode => Instance._generationMode;

    public enum GenerationMode
    {
        StaticSize,
        TargetPlayers,
        MultiplayerServerSide, // TBA
        MultiplayerClientUnsafe, // TBA
        MultiplayerClientSafe // is it k-dot is it aubrey or me
    }

    public GenerationMode _generationMode;

    #region StaticSize Settings
    public static Vector3Int StaticWorldSize => Instance._staticWorldSize;
    [ShowInInspector, ShowIf("_generationMode", GenerationMode.StaticSize)] public Vector3Int _staticWorldSize = new Vector3Int(4, 2, 4);
    #endregion

    #region TargetPlayers Settings

    [ShowInInspector, ShowIf("_generationMode", GenerationMode.TargetPlayers)] public List<Transform> targetPlayers = new List<Transform>();

    #endregion

    #region MultiplayerServerSide Settings
    


    #endregion

    #region MultiplayerClientSide Settings

    #endregion

    [Header("World Settings")]
    public int setSeed = 0;
    private int _seed;

    public int chunkSize = 16;
    public int renderDistanceHorizontal = 8, renderDistanceVertical = 8;
    public int numPointsPerAxis = 8;

    public const int threadGroupSize = 8;

    public struct WorldSettings
    {
        public int chunkSize;
        public int renderDistanceHorizontal;
        public int renderDistanceVertical;

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

        _worldSettings = new WorldSettings
        {
            chunkSize = chunkSize,
            renderDistanceHorizontal = renderDistanceHorizontal,
            renderDistanceVertical = renderDistanceVertical,

            // Shared values
            numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis,
            numPointsPerAxis = numPointsPerAxis,
            numVoxelsPerAxis = numVoxelsPerAxis,
            numVoxels = numVoxels,
            maxTriangleCount = maxTriangleCount,
            numThreadsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float) threadGroupSize),
            boundsSize = chunkSize,
            pointSpacing = chunkSize / ((float)numPointsPerAxis - 1),
            worldBounds = new Vector3(renderDistanceHorizontal, renderDistanceVertical, renderDistanceHorizontal) * chunkSize,

            threadGroupSize = threadGroupSize
        };
    }
}