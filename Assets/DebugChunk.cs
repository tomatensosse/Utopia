using Unity.Mathematics;
using UnityEngine;

public class DebugChunk : MonoBehaviour
{
    public int size = 8;
    public int trisPerAxis = 8;
    public Color chunkBoundsColor = Color.white;

    public Material material;

    public GameObject heightGen;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    [Header("Params")]
    [SerializeField] float noiseScale = 1f;
    [SerializeField] float heightChange = 0f;
    [SerializeField] int octaves = 1;
    [SerializeField] int offset;

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
        
        noiseGen.OverrideParameters(noiseScale, heightChange, octaves, offset, amplitude, lacunarity, heightChangeVariation);

        MeshData data = noiseGen.Execute(size, Vector2.zero, 0);

        Mesh mesh = new Mesh();
        mesh.name = "Chunk Mesh";

        mesh.vertices = data.vertices.ToArray();
        mesh.triangles = data.triangles;

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        meshRenderer.material = material;
    }

    private void Initialize()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }
}
