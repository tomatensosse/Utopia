using UnityEngine;

public class RidgedNoiseNode : DensityNode
{
    [Header("Ridged Noise Parameters")]
    public float lacunarity = 4;
    public float persistence = 6;
    public float noiseScale = 1;
    public float ridgeWeight = 1;

    public override ComputeBuffer GenerateDensity(DebugChunk debugChunk)
    {
        DynamicParameters dynamicParameters = GenerateDynamicParameters(debugChunk.GetDynamicParameterInput());

        SetBaseParameters(debugChunk.GetBaseParameters());
        SetDynamicParameters(dynamicParameters);

        SetRidgedNoiseParameters();

        Dispatch(dynamicParameters.numThreadsPerAxis);

        if (buffersToRelease != null) {
            foreach (var b in buffersToRelease) {
                b.Release();
            }
        }

        return pointsBuffer;
    }

    public void SetRidgedNoiseParameters()
    {
        shader.SetFloat("lacunarity", lacunarity);
        shader.SetFloat("persistence", persistence);
        shader.SetFloat("noiseScale", noiseScale);
        shader.SetFloat("ridgeWeight", ridgeWeight);
    }
}