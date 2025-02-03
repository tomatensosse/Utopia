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
    public ComputeBuffer BlendedBuffer => finalizedBlendedBuffer;
    protected ComputeBuffer finalizedBlendedBuffer;
    public ComputeBuffer OngoingBlendedBuffer => ongoingBlendedBuffer;
    protected ComputeBuffer ongoingBlendedBuffer;
    public bool isDensityGenerated = false;
    public bool isMeshGenerated = false;
    public bool isBlended = false;
    public bool blendCanidate = false;
    public Dictionary<Vector3Int, Chunk> blendNeighbors = new Dictionary<Vector3Int, Chunk>();
    public Dictionary<Vector3Int, Chunk> neighbors = new Dictionary<Vector3Int, Chunk>();
    public List<Vector3Int> blendOffsets = new List<Vector3Int>();
    public List<Vector3Int> brokenOffsets = new List<Vector3Int>();

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

        isDensityGenerated = true;
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
            if (finalizedBlendedBuffer == null)
            {
                Debug.LogError($"Chunk({chunkPosition}) | Mesh is blended but no blendedBuffer?!");
                return;
            }
            
            mesh = MeshGenerator.Instance.GenerateMesh(finalizedBlendedBuffer, 1);

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

        isMeshGenerated = true;
    }

    public void BlendWithNeighbors(Dictionary<Vector3Int, Chunk> blendNeighbors)
    {
        ongoingBlendedBuffer = densityBuffer;

        foreach (var blendNeighbor in blendNeighbors)
        {
            if (blendNeighbor.Value.blendOffsets.Contains(-blendNeighbor.Key))
            {
                continue;
            }

            // Blend with neighbor
        }

        foreach (var neighbor in neighbors)
        {
            if (!blendNeighbors.ContainsKey(neighbor.Key))
            {
                brokenOffsets.Add(neighbor.Key);
            }
        }

        foreach (Vector3Int brokenOffset in brokenOffsets)
        {
            
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
            ongoingBlendedBuffer, 
            neighborChunk.DensityBuffer,
            neighborRelative
        );

        blendOffsets.Add(neighborRelative);
        neighborChunk.blendOffsets.Add(-neighborRelative);

        this.ongoingBlendedBuffer = blendedBuffer;

        Debug.Log($"Chunk({chunkPosition}) | Blended with neighbor at {neighborRelative} | NeighborChunk Pos : {neighborChunk.chunkPosition}; NeighborRelative: {neighborRelative}");
    }

    public bool OffsetBlended(Vector3Int offset)
    {
        return blendOffsets.Contains(offset);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (biome == null)
        {
            Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
        }
        else if (!isDensityGenerated)
        {
            Gizmos.color = biome.biomeColor;
        }
        else
        {
            Gizmos.color = new Color(biome.biomeColor.r, biome.biomeColor.g, biome.biomeColor.b, 0.3f);
        }

        if (!isDensityGenerated)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(World.Settings.chunkSize, World.Settings.chunkSize, World.Settings.chunkSize));
        }
        else if (!isMeshGenerated)
        {
            Gizmos.DrawCube(transform.position, new Vector3(World.Settings.chunkSize, World.Settings.chunkSize, World.Settings.chunkSize));
        }
    }
}