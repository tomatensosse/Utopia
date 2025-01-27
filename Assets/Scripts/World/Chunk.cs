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
    protected ComputeBuffer blendedBuffer;
    protected bool isBlended = false;

    public Dictionary<Vector3Int, float> debugInspectorDensities => _debugInspectorDensities;
    private Dictionary<Vector3Int, float> _debugInspectorDensities = new Dictionary<Vector3Int, float>();

    [Header("Components")]
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    void OnDestroy()
    {
        if (densityBuffer != null) densityBuffer.Release();
        if (blendedBuffer != null) blendedBuffer.Release();
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

    public void BlendDensity(Dictionary<Vector3Int, Chunk> neighbors)
    {
        if (neighbors.Any(n => n.Value.biome != biome)) {
            var blendedBuffer = BiomeBlender.Instance.BlendWithNeighbors(this, neighbors);

            this.blendedBuffer = blendedBuffer;
            isBlended = true;
        }
    }

    public void GenerateMesh()
    {
        Mesh mesh = new Mesh();

        if (isBlended)
        {
            mesh = MeshGenerator.Instance.GenerateMesh(blendedBuffer, 1);
        }
        if (!isBlended)
        {
            mesh = MeshGenerator.Instance.GenerateMesh(densityBuffer, 1);
        }

        meshFilter.mesh = mesh;
        meshFilter.sharedMesh = mesh;

        meshRenderer.material = biome.material;
        meshRenderer.sharedMaterial = biome.material;

        meshCollider.sharedMesh = mesh;

        densityBuffer.Release();

        if (isBlended)
        {
            blendedBuffer.Release();
        }
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