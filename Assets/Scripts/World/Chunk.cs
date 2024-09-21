using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Color chunkBoundsColor;

    public Biome chunkBiome;

    public int chunkSizeHorizontal;

    public int chunkSizeVertical;

    public Vector2Int chunkPosition;

    [HideInInspector] public bool setUp = false;

    [HideInInspector] public bool meshIsSet = false;

    MeshFilter meshFilter;

    MeshRenderer meshRenderer;

    MeshCollider meshCollider;

    [HideInInspector] public bool leftEdgeInterpolated = false;
    [HideInInspector] public bool rightEdgeInterpolated = false;
    [HideInInspector] public bool frontEdgeInterpolated = false;
    [HideInInspector] public bool backEdgeInterpolated = false;

    [HideInInspector] public bool frontLeftEdgeInterpolated = false;
    [HideInInspector] public bool frontRightEdgeInterpolated = false;
    [HideInInspector] public bool backLeftEdgeInterpolated = false;
    [HideInInspector] public bool backRightEdgeInterpolated = false;

    public bool generateColliders = false;

    public bool generateablesHaveBeenGenerated = false;

    public void SetMesh(List<Vector3> vertices, int[] triangles)
    {
        if (!setUp)
        {
            meshFilter = this.AddComponent<MeshFilter>();

            if (generateColliders)
            {
                meshCollider = this.AddComponent<MeshCollider>();
                meshRenderer = this.AddComponent<MeshRenderer>();
            }

            setUp = true;
        }

        Mesh mesh = new Mesh();
        mesh.name = "Chunk Mesh";

        mesh.Clear();

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);

        this.gameObject.layer = LayerMask.NameToLayer("Ground");

        meshFilter.mesh = mesh;

        mesh.RecalculateNormals();

        if (generateColliders)
        {
            meshCollider.sharedMesh = mesh;

            meshCollider.convex = true;
        }

        meshIsSet = true;
    }

    public void Configure(Material mat)
    {
        GetComponent<MeshRenderer>().material = mat;
    }

    public void PrintVectors()
    {
        foreach (Vector3 vec in meshFilter.sharedMesh.vertices)
        {
            Debug.Log(vec);
        }
    }

    public void PrintTriangles()
    {
        foreach (int tri in meshFilter.mesh.triangles)
        {
            Debug.Log(tri);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = chunkBoundsColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(chunkSizeHorizontal, chunkSizeVertical, chunkSizeHorizontal));
    }

    public Mesh GetMesh()
    {
        return meshFilter.mesh;
    }

    public ChunkData GetChunkData()
    {
        ChunkData data = new ChunkData();

        data.vertices = new List<Vector3>(meshFilter.mesh.vertices);
        data.triangles = meshFilter.mesh.triangles;

        // Add entities and spawnables to the data

        return data;
    }
}