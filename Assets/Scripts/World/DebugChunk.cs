// WAS USED FOR DEBUGGING PURPOSES ONLY

/*
using Sirenix.OdinInspector;
using UnityEngine;

public class DebugChunk : MonoBehaviour
{
    public static DebugChunk Instance { get; private set; }

    [Header("Components")]
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    [Header("Biome")]
    public Biome biome;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public void Generate() // Dont forget to release the density/points buffer for memory leaks
    {
        ComputeBuffer densityBuffer = biome.root.GenerateDensity(this);

        Mesh mesh = MeshGenerator.Instance.GenerateMesh(densityBuffer, 1);

        meshFilter.mesh = mesh;
        meshFilter.sharedMesh = mesh;

        meshRenderer.material = biome.material;
        meshRenderer.sharedMaterial = biome.material;

        meshCollider.sharedMesh = mesh;

        densityBuffer.Release();
    }

    public DensityNode.BaseParameters GetBaseParameters()
    {
        return new DensityNode.BaseParameters
        {
            boundsSize = World.Settings.chunkSize,
            spacing = World.Settings.pointSpacing,
            worldSize = World.Settings.worldBounds,
        };
    }

    public DensityNode.DynamicParameterInput GetDynamicParameterInput()
    {
        return new DensityNode.DynamicParameterInput
        {
            seed = World.Seed,
            numPointsPerAxis = World.Settings.numPointsPerAxis,
            threadGroupSize = World.Settings.threadGroupSize,
            centre = transform.position
        };
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(World.Settings.chunkSize, World.Settings.chunkSize, World.Settings.chunkSize));
    }
}
*/