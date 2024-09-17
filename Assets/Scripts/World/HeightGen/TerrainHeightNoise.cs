using System.Collections.Generic;
using UnityEngine;

public class TerrainHeightNoise : TerrainHeightGen
{
    
    [SerializeField] float noiseScale = 1f; // The smaller the scale values, the larger the features will be on the terrain
    [SerializeField] float heightChange = 0f;
    [SerializeField] int octaves = 1;
    [SerializeField] int offset;

    [SerializeField] float amplitude = 1;

    [SerializeField] float lacunarity = 2f; // Variables to determine the variation of scale and height change over multiple runs
    [SerializeField] float heightChangeVariation = 0.5f;

    public override MeshData Execute(int chunkSizeHorizontal, Vector2 chunkPosition, float seed)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float scale = noiseScale / 10f;

        for (int x = 0; x < (chunkSizeHorizontal + 1); x++)
        {
            for (int z = 0; z < (chunkSizeHorizontal + 1); z++)
            {
                float finalVal = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float noise = Mathf.PerlinNoise((x + chunkPosition.x + seed) * scale * Mathf.Pow(lacunarity, i), (z + chunkPosition.y + seed) * scale * Mathf.Pow(lacunarity, i));
                    noise *= amplitude;
                    finalVal += noise;
                }

                vertices.Add(new Vector3(x - (chunkSizeHorizontal + 1) / 2, finalVal, z - (chunkSizeHorizontal + 1) / 2));
            }
        }

        // OPTİMİZE ET VE BU KISMI BİRDEN FAZLA KEZ ÇALIŞTIRMA !!! !!! !!! ÇİĞDEMİ ÇOK SEVİYORUM !!! GÖTÜ DE DEVASA KALBİ DE !!!
        for (int x = 0; x < chunkSizeHorizontal; x++)
        {
            for (int z = 0; z < chunkSizeHorizontal; z++)
            {
                int topLeft = x + z * (chunkSizeHorizontal + 1);
                int topRight = (x + 1) + z * (chunkSizeHorizontal + 1);
                int bottomLeft = x + (z + 1) * (chunkSizeHorizontal + 1);
                int bottomRight = (x + 1) + (z + 1) * (chunkSizeHorizontal + 1);

                triangles.Add(topLeft);
                triangles.Add(topRight);
                triangles.Add(bottomLeft);

                triangles.Add(topRight);
                triangles.Add(bottomRight);
                triangles.Add(bottomLeft);
            }
        }

        MeshData meshData = new MeshData();
        meshData.vertices = vertices;
        meshData.triangles = triangles.ToArray();

        return meshData;
    }

    public void OverrideParameters(float noiseScale, float heightChange, int octaves, int offset, float amplitude, float lacunarity, float heightChangeVariation)
    {
        this.noiseScale = noiseScale;
        this.heightChange = heightChange;
        this.octaves = octaves;
        this.offset = offset;
        this.amplitude = amplitude;
        this.lacunarity = lacunarity;
        this.heightChangeVariation = heightChangeVariation;
    }
}