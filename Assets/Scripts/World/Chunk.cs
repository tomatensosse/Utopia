using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Header("Chunk Parameters")]
    public Vector3Int chunkPosition;
    public Biome biome;

    public ComputeBuffer DensityBuffer => densityBuffer;
    protected ComputeBuffer densityBuffer;
    public ComputeBuffer BlendedBuffer => blendedBuffer;
    protected ComputeBuffer blendedBuffer;
    protected ComputeBuffer ongoingBlendedBuffer;
    public bool isBlended = false;

    [Header("Components")]
    [HideInInspector] public MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    public struct FaceDensities
    {
        public ComputeBuffer densityBuffer;
        public int width;
        public int depth;
        public int blendDepth;
        public Vector3Int startPoint;
    }

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

    public void GenerateDensity() // Dont forget to release the density/points buffer for memory leaks
    {
        densityBuffer = biome.root.GenerateDensity(transform.position);
    }

    public void GenerateMesh(bool releaseAfterComplete)
    {
        Mesh mesh;
        
        if (!isBlended)
        {
            mesh = MeshGenerator.Instance.GenerateMesh(densityBuffer, 1);

            Debug.Log($"Chunk({chunkPosition}) | Generated mesh");
        }
        else
        {
            if (blendedBuffer == null)
            {
                Debug.LogError($"Chunk({chunkPosition}) | Mesh is blended but no blendedBuffer?!");
                return;
            }
            
            mesh = MeshGenerator.Instance.GenerateMesh(blendedBuffer, 1);

            Debug.Log($"Chunk({chunkPosition}) | Generated mesh from blended buffer");
        }

        if (mesh == null || mesh.vertexCount < 3)
        {
            Debug.LogWarning($"Chunk({chunkPosition}) | Invalid mesh generated for chunk at");
            return;
        }

        meshFilter.mesh = mesh;
        meshFilter.sharedMesh = mesh;
        meshRenderer.material = biome.material;
        meshRenderer.sharedMaterial = biome.material;
        
        // Only set collider if mesh is valid
        if (mesh.vertexCount >= 3)
        {
            meshCollider.sharedMesh = mesh;
        }

        if (releaseAfterComplete)
        {
            densityBuffer.Release();
        }
    }

    public void BlendWithNeighbor(Chunk neighborChunk, Vector3Int neighborRelative)
    {
        if (densityBuffer == null || neighborChunk.DensityBuffer == null)
        {
            Debug.LogError("Cannot blend - density buffers not available");
            return;
        }

        var blendedBuffer = BlendGenerator.Instance.BlendWithNeighbor(
            densityBuffer, 
            neighborChunk.DensityBuffer,
            neighborRelative
        );

        this.blendedBuffer = blendedBuffer;
        isBlended = true;

        Debug.Log($"Chunk({chunkPosition}) | Blended with neighbor at {neighborRelative} | NeighborChunk Pos : {neighborChunk.chunkPosition}; NeighborRelative: {neighborRelative}");
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