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

    public void GenerateDensity() // Dont forget to release the density/points buffer for memory leaks
    {
        densityBuffer = biome.root.GenerateDensity(transform.position);
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
}