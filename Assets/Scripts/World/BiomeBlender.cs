using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BiomeBlender : MonoBehaviour
{
    public static BiomeBlender Instance { get; private set; }

    [SerializeField] private ComputeShader blendShader;
    private ComputeBuffer neighborPositionsBuffer;
    private ComputeBuffer hasNeighborBuffer;
    private ComputeBuffer allNeighborPointsBuffer;

    [Range(1, 10)] public float blendDistanceMultiplier = 5f;
    [Range(1, 10)] public float blendStrengthMultiplier = 2f;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public ComputeBuffer BlendWithNeighbors(Chunk centerChunk, Dictionary<Vector3Int, Chunk> neighborChunks)
    {
        int numPoints = World.Settings.numPointsPerAxis;
        int pointsPerChunk = numPoints * numPoints * numPoints;
        
        int[] hasNeighbor = new int[26];
        Vector3[] neighborPositions = new Vector3[26];
        float[] allNeighborPoints = new float[26 * pointsPerChunk * 4];

        int i = 0;
        foreach (var chunk in neighborChunks.Values)
        {
            if (chunk.biome != centerChunk.biome)
            {
                hasNeighbor[i] = 1;
                neighborPositions[i] = chunk.transform.position;
                
                float[] chunkPoints = new float[pointsPerChunk * 4];
                chunk.DensityBuffer.GetData(chunkPoints);
                System.Array.Copy(chunkPoints, 0, allNeighborPoints, i * pointsPerChunk * 4, pointsPerChunk * 4);
            }
            i++;
        }

        var blendedPoints = new ComputeBuffer(pointsPerChunk, sizeof(float) * 4);

        UpdateBuffers(neighborPositions, hasNeighbor, allNeighborPoints, pointsPerChunk);
        
        blendShader.SetBuffer(0, "centerChunkPoints", centerChunk.DensityBuffer);
        blendShader.SetBuffer(0, "blendedPoints", blendedPoints);
        blendShader.SetBuffer(0, "allNeighborPoints", allNeighborPointsBuffer);
        blendShader.SetBuffer(0, "neighborPositions", neighborPositionsBuffer);
        blendShader.SetBuffer(0, "hasNeighbor", hasNeighborBuffer);
        
        blendShader.SetInt("numNeighbors", neighborChunks.Count);
        blendShader.SetInt("numPointsPerAxis", numPoints);
        blendShader.SetFloat("blendDistance", World.Settings.chunkSize * blendDistanceMultiplier);
        blendShader.SetFloat("blendStrength", 1f * blendStrengthMultiplier);

        blendShader.Dispatch(0, Mathf.CeilToInt(numPoints / 8f), Mathf.CeilToInt(numPoints / 8f), Mathf.CeilToInt(numPoints / 8f));

        ReleaseBuffers();
        return blendedPoints;
    }

    private void UpdateBuffers(Vector3[] positions, int[] hasNeighbor, float[] allPoints, int pointsPerChunk)
    {
        neighborPositionsBuffer = new ComputeBuffer(26, sizeof(float) * 3);
        hasNeighborBuffer = new ComputeBuffer(26, sizeof(int));
        allNeighborPointsBuffer = new ComputeBuffer(26 * pointsPerChunk, sizeof(float) * 4);

        neighborPositionsBuffer.SetData(positions);
        hasNeighborBuffer.SetData(hasNeighbor);
        allNeighborPointsBuffer.SetData(allPoints);
    }

    private void ReleaseBuffers()
    {
        neighborPositionsBuffer?.Release();
        hasNeighborBuffer?.Release();
        allNeighborPointsBuffer?.Release();
    }

    void OnDestroy()
    {
        ReleaseBuffers();
    }
}