using System.Collections.Generic;
using Unity.AI.Navigation;
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
    NavMeshSurface navMeshSurface;

    [HideInInspector] public bool leftEdgeInterpolated = false;
    [HideInInspector] public bool rightEdgeInterpolated = false;
    [HideInInspector] public bool frontEdgeInterpolated = false;
    [HideInInspector] public bool backEdgeInterpolated = false;

    [HideInInspector] public bool frontLeftEdgeInterpolated = false;
    [HideInInspector] public bool frontRightEdgeInterpolated = false;
    [HideInInspector] public bool backLeftEdgeInterpolated = false;
    [HideInInspector] public bool backRightEdgeInterpolated = false;

    public bool generateColliders = false;

    public List<GameObject> spawnablesInChunk = new List<GameObject>();
    public bool spawnablesHaveBeenGenerated = false;

    public void SetMesh(List<Vector3> vertices, int[] triangles)
    {
        meshFilter = GetComponent<MeshFilter>();

        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        meshCollider = GetComponent<MeshCollider>();

        if (meshCollider == null && generateColliders)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }

        navMeshSurface = GetComponent<NavMeshSurface>();

        if (navMeshSurface == null)
        {
            navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
        }

        Mesh mesh = new Mesh();
        mesh.name = "Chunk Mesh";

        mesh.Clear();

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);

        mesh.RecalculateNormals();

        mesh.RecalculateTangents();
    
        mesh.RecalculateBounds();

        Vector2[] uvs = new Vector2[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            // Normalize UVs based on the X and Z coordinates of each vertex
            float u = (vertices[i].x + chunkSizeHorizontal / 2f) / chunkSizeHorizontal;
            float v = (vertices[i].z + chunkSizeHorizontal / 2f) / chunkSizeHorizontal;
            uvs[i] = new Vector2(u, v);
        }
        mesh.uv = uvs;

        meshFilter.mesh = mesh;

        this.gameObject.layer = LayerMask.NameToLayer("Ground");

        if (generateColliders)
        {
            meshCollider.sharedMesh = mesh;

            meshCollider.convex = true;
        }

        meshIsSet = true;

        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
    }

    public void Configure(Material mat)
    {
        GetComponent<MeshRenderer>().material = mat;
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

    public void DestroyChunk()
    {
        Destroy(gameObject);
    }

    public ChunkData GetChunkData()
    {
        ChunkData data = new ChunkData();

        data.vertices = new List<Vector3>(meshFilter.mesh.vertices);
        data.triangles = meshFilter.mesh.triangles;

        data.chunkPosition = chunkPosition;

        if (data.spawnableDatas == null)
        {
            data.spawnableDatas = new List<SpawnableData>();
        }

        foreach (GameObject spawnable in spawnablesInChunk)
        {
            Spawnable spawnableComponent = spawnable.GetComponent<Spawnable>();
            SpawnableData spawnableData = spawnableComponent.GetSpawnableData();

            if (spawnableData != null)
            {
                data.spawnableDatas.Add(spawnableData);
            }
        }

        return data;
    }

    public Vector3 GetRandomPosition()
    {
        Vector3 position = meshFilter.mesh.vertices[Random.Range(0, meshFilter.mesh.vertices.Length)];

        return position;
    }
}