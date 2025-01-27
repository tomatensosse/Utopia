using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Header("Chunk Parameters")]
    public Vector3Int chunkPosition;
    public Biome biome;

    public ComputeBuffer DensityBuffer => densityBuffer;
    protected ComputeBuffer densityBuffer;
    protected bool isBlended = false;

    public Dictionary<Vector3Int, float> debugInspectorDensities => _debugInspectorDensities;
    private Dictionary<Vector3Int, float> _debugInspectorDensities = new Dictionary<Vector3Int, float>();

    [Header("Components")]
    [HideInInspector] public MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    void OnDestroy()
    {
        if (densityBuffer != null) densityBuffer.Release();
    }

    public void Initialize()
    {
        meshFilter = this.AddComponent<MeshFilter>();
        meshRenderer = this.AddComponent<MeshRenderer>();
        meshCollider = this.AddComponent<MeshCollider>();
    }

    public void GenerateDensity(bool outputDebugInspectorDensities = false) // Dont forget to release the density/points buffer for memory leaks
    {
        densityBuffer = biome.root.GenerateDensity(transform.position);

        if (outputDebugInspectorDensities)
        {
            _debugInspectorDensities = new Dictionary<Vector3Int, float>();

            float[] densityArray = new float[World.Settings.numPoints];
            densityBuffer.GetData(densityArray);

            for (int x = 0; x < World.Settings.numPointsPerAxis; x++)
            {
                for (int y = 0; y < World.Settings.numPointsPerAxis; y++)
                {
                    for (int z = 0; z < World.Settings.numPointsPerAxis; z++)
                    {
                        Vector3Int point = new Vector3Int(x, y, z);
                        _debugInspectorDensities.Add(point, densityArray[x + y * World.Settings.numPointsPerAxis + z * World.Settings.numPointsPerAxis * World.Settings.numPointsPerAxis]);
                    }
                }
            }
        }
    }

    public void GenerateMesh(bool blendMesh = false)
    {
        if (densityBuffer == null)
        {
            Debug.LogError($"Density buffer is null for chunk at {chunkPosition}");
            return;
        }

        Mesh mesh = new Mesh();

        try
        {
            if (!blendMesh)
            {
                mesh = MeshGenerator.Instance.GenerateMesh(densityBuffer, 1);
            }
            else
            {
                // Ensure we have neighbors before attempting blend
                var neighbors = ChunkGenerator.Instance.GetNeighborChunks(chunkPosition);
                if (neighbors != null && neighbors.Count > 0)
                {
                    List<ComputeBuffer> neighborBuffers = GetNeighborDensityBuffers();
                    if (neighborBuffers != null && neighborBuffers.Count > 0)
                    {
                        mesh = BlendedMeshGenerator.Instance.GenerateBlendedMesh(densityBuffer, 1, neighborBuffers);
                    }
                    else
                    {
                        // Fallback to regular mesh generation if no valid neighbor buffers
                        mesh = MeshGenerator.Instance.GenerateMesh(densityBuffer, 1);
                    }
                }
                else
                {
                    // Fallback to regular mesh generation if no neighbors
                    mesh = MeshGenerator.Instance.GenerateMesh(densityBuffer, 1);
                }
            }

            if (mesh != null)
            {
                meshFilter.mesh = mesh;
                meshFilter.sharedMesh = mesh;
                meshRenderer.material = biome.material;
                meshRenderer.sharedMaterial = biome.material;
                meshCollider.sharedMesh = mesh;
            }
        }
        finally
        {
            if (densityBuffer != null)
            {
                densityBuffer.Release();
            }
        }
    }

    private List<ComputeBuffer> GetNeighborDensityBuffers()
    {
        List<ComputeBuffer> neighborBuffers = new List<ComputeBuffer>();
        Dictionary<Vector3Int, Chunk> neighbors = ChunkGenerator.Instance.GetNeighborChunks(chunkPosition);

        // Process neighbors in a fixed order to match shader's expectation
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0) continue;

                    Vector3Int neighborPos = chunkPosition + new Vector3Int(x, y, z);
                    if (neighbors.TryGetValue(neighborPos, out Chunk neighbor) && 
                        neighbor != null && 
                        neighbor.DensityBuffer != null)
                    {
                        neighborBuffers.Add(neighbor.DensityBuffer);
                    }
                    else
                    {
                        // Add a dummy buffer to maintain order
                        var dummyBuffer = new ComputeBuffer(1, sizeof(float) * 4);
                        dummyBuffer.SetData(new Vector4[1] { Vector4.zero });
                        neighborBuffers.Add(dummyBuffer);
                    }
                }
            }
        }

        return neighborBuffers;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || biome == null)
        {
            return;
        }

        Gizmos.color = biome.biomeColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(World.Settings.chunkSize, World.Settings.chunkSize, World.Settings.chunkSize));
    }

    public bool PointInChunk(Vector3Int point)
    {
        return _debugInspectorDensities.ContainsKey(point);
    }

    public float GetDensity(Vector3Int point)
    {
        return _debugInspectorDensities[point];
    }
}