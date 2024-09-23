using System.Collections.Generic;
using UnityEngine;

public class TerrainHeightNoise : TerrainHeightGen
{
    [SerializeField] float noiseScale = 1f; 
    [SerializeField] float amplitude = 1;
    [SerializeField] int octaves = 1;
    [SerializeField] float lacunarity = 2f;

    public override MeshData Execute(int chunkSizeHorizontal, int pointsPerAxis, Vector2 chunkPosition, float seed)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float pointSpacing = (float)chunkSizeHorizontal / (pointsPerAxis - 1); // Proper spacing

        // Generate vertices
        for (int x = 0; x < pointsPerAxis; x++)
        {
            for (int z = 0; z < pointsPerAxis; z++)
            {
                float finalVal = 0;

                // Octave-based noise generation
                for (int i = 0; i < octaves; i++)
                {
                    float noiseX = (x * pointSpacing + chunkPosition.x + seed) * noiseScale * Mathf.Pow(lacunarity, i);
                    float noiseZ = (z * pointSpacing + chunkPosition.y + seed) * noiseScale * Mathf.Pow(lacunarity, i);

                    float noise = Mathf.PerlinNoise(noiseX, noiseZ);
                    noise *= amplitude;
                    finalVal += noise;
                }

                // Add vertex
                vertices.Add(new Vector3(x * pointSpacing - chunkSizeHorizontal / 2f, finalVal, z * pointSpacing - chunkSizeHorizontal / 2f));
            }
        }

        // Generate triangles (two per quad)
        for (int x = 0; x < pointsPerAxis - 1; x++)
        {
            for (int z = 0; z < pointsPerAxis - 1; z++)
            {
                int topLeft = x + z * pointsPerAxis;
                int topRight = (x + 1) + z * pointsPerAxis;
                int bottomLeft = x + (z + 1) * pointsPerAxis;
                int bottomRight = (x + 1) + (z + 1) * pointsPerAxis;

                // Ensure clockwise order for triangles to face upward

                // First triangle (clockwise)
                triangles.Add(topLeft);
                triangles.Add(topRight);
                triangles.Add(bottomLeft);

                // Second triangle (clockwise)
                triangles.Add(topRight);
                triangles.Add(bottomRight);
                triangles.Add(bottomLeft);
            }
        }

        // Create and return mesh data
        MeshData meshData = new MeshData();
        meshData.vertices = vertices;
        meshData.triangles = triangles.ToArray();

        return meshData;
    }

    public void OverrideParameters(float noiseScale, float amplitude, int octaves, float lacunarity)
    {
        this.noiseScale = noiseScale;
        this.amplitude = amplitude;
        this.octaves = octaves;
        this.lacunarity = lacunarity;
    }
}