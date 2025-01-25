using Unity.VisualScripting;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Header("Chunk Parameters")]
    public Vector3Int chunkPoisiton;
    public Biome biome;

    [Header("Components")]
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    public void Initialize()
    {
        meshFilter = this.AddComponent<MeshFilter>();
        meshRenderer = this.AddComponent<MeshRenderer>();
        meshCollider = this.AddComponent<MeshCollider>();
    }

    public void Generate() // Dont forget to release the density/points buffer for memory leaks
    {
        ComputeBuffer densityBuffer = biome.root.GenerateDensity(transform.position);

        Mesh mesh = MeshGenerator.Instance.GenerateMesh(densityBuffer, 1);

        meshFilter.mesh = mesh;
        meshFilter.sharedMesh = mesh;

        meshRenderer.material = biome.material;
        meshRenderer.sharedMaterial = biome.material;

        meshCollider.sharedMesh = mesh;

        densityBuffer.Release();
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