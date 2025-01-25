using UnityEngine;

public class SimpleNoiseNode : DensityNode
{
    [Header("Simple Noise Parameters")]
    public float lacunarity;
    public float persistence;
    public float noiseScale;
    public float noiseWeight;
    public bool closeEdges;
    public float floorOffset;
    public float weightMultiplier;
    public float hardFloor;
    public float hardFloorWeight;

    public override ComputeBuffer GenerateDensity(DebugChunk debugChunk)
    {
        DynamicParameters dynamicParameters = GenerateDynamicParameters(debugChunk.GetDynamicParameterInput());

        SetBaseParameters(debugChunk.GetBaseParameters());
        SetDynamicParameters(dynamicParameters);

        SetSimpleNoiseParameters();

        Dispatch(dynamicParameters.numThreadsPerAxis);

        if (buffersToRelease != null) {
            foreach (var b in buffersToRelease) {
                b.Release();
            }
        }

        LogBuffer(pointsBuffer);

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

    private void LogBuffer(ComputeBuffer buffer)
    {
        Vector4[] data = new Vector4[buffer.count];
        buffer.GetData(data);

        string s = "";

        for (int i = 0; i < data.Length; i++)
        {
            s += data[i].ToString() + "\n";
        }

        Debug.Log(s);
    }
}