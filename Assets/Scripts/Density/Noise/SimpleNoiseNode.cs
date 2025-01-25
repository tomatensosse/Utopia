using UnityEngine;

public class SimpleNoiseNode : DensityNode
{
    [Header("Simple Noise Parameters")]
    public float lacunarity = 8;
    public float persistence = 8;
    public float noiseScale = .5f;
    public float noiseWeight = 1;
    public bool closeEdges = false; // Add to other noises :3
    public float floorOffset; 
    public float weightMultiplier = 1;
    public float hardFloor = 1;
    public float hardFloorWeight = 1;

    public override ComputeBuffer GenerateDensity(Vector3 worldPositionForChunk)
    {
        GenerateDynamicParameters();

        SetBaseParameters();
        SetDynamicParameters();

        SetSimpleNoiseParameters();

        Dispatch(worldPositionForChunk);

        if (buffersToRelease != null) {
            foreach (var b in buffersToRelease) {
                b.Release();
            }
        }

        return pointsBuffer;
    }

    public void SetSimpleNoiseParameters()
    {
        shader.SetFloat("lacunarity", lacunarity);
        shader.SetFloat("persistence", persistence);
        shader.SetFloat("noiseScale", noiseScale);
        shader.SetFloat("noiseWeight", noiseWeight);
        shader.SetBool("closeEdges", closeEdges);
        shader.SetFloat("floorOffset", floorOffset);
        shader.SetFloat("weightMultiplier", weightMultiplier);
        shader.SetFloat("hardFloor", hardFloor);
        shader.SetFloat("hardFloorWeight", hardFloorWeight);
    }
}