using Unity.Mathematics;
using UnityEngine;

public class DebugChunk : MonoBehaviour
{
    public Color chunkBoundsColor = Color.white;

    public Material material;

    public GameObject heightGen;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    [Header("Params")]
    [SerializeField] int size = 8;
    [SerializeField] int pointsPerAxis = 8;
    [SerializeField] float noiseScale = 1f;
    [SerializeField] int octaves = 1;

    [SerializeField] float amplitude = 1f;

    [SerializeField] float lacunarity = 2f; // Variables to determine the variation of scale and height change over multiple runs
    [SerializeField] float heightChangeVariation = 0.5f;

    public void Start()
    {
        Initialize();
        InvokeRepeating("Generate", 0, 1);
    }

    private void Generate()
    {
        TerrainHeightNoise noiseGen = heightGen.GetComponent<TerrainHeightNoise>();
        
        noiseGen.OverrideParameters(noiseScale, amplitude, octaves, lacunarity);

        MeshData data = noiseGen.Execute(size, pointsPerAxis, Vector2.zero, 0);

        Mesh mesh = new Mesh();
        mesh.name = "Chunk Mesh";

        mesh.vertices = data.vertices.ToArray();
        mesh.triangles = data.triangles;

        mesh.RecalculateNormals();

        mesh.RecalculateTangents();

        mesh.RecalculateBounds();

        Vector2[] uvs = new Vector2[data.vertices.Count];
        for (int i = 0; i < data.vertices.Count; i++)
        {
            // Normalize UVs based on the X and Z coordinates of each vertex
            float u = (data.vertices[i].x + size / 2f) / size;
            float v = (data.vertices[i].z + size / 2f) / size;
            uvs[i] = new Vector2(u, v);
        }
        mesh.uv = uvs;

        meshFilter.mesh = mesh;

        meshCollider.sharedMesh = mesh;

        meshCollider.convex = true;
    }

    private void Initialize()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
    }
}
