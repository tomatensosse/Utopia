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

    public bool isDensityGenerated = false;
    public bool isMeshGenerated = false;

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

    public virtual void GenerateDensity() // Dont forget to release the density/points buffer for memory leaks
    {
        densityBuffer = biome.root.GenerateDensity(transform.position);

        isDensityGenerated = true;
    }

    public void GenerateMesh()
    {
        Mesh mesh;
        
        mesh = MeshGenerator.Instance.GenerateMesh(densityBuffer, 1);

        meshFilter.mesh = mesh;
        meshFilter.sharedMesh = mesh;
        meshRenderer.material = biome.material;
        meshRenderer.sharedMaterial = biome.material;
        
        // Only set collider if mesh is valid
        if (mesh.vertexCount >= 3)
        {
            meshCollider.sharedMesh = mesh;
        }

        densityBuffer.Release();

        isMeshGenerated = true;
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