using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DensityNode : Node
{
    public ComputeShader shader;
    public ComputeBuffer pointsBuffer;

    [Header("Base Parameters")]
    public Vector3 offset = Vector4.zero; // FIX
    public Vector4 parameters = new Vector4(1, 0, 0, 1); // FIX
    public int numOctaves = 2;

    protected List<ComputeBuffer> buffersToRelease = new List<ComputeBuffer>();

    public struct BaseParameters
    {
        public float boundsSize;
        public float spacing;
        public Vector3 worldSize;
    }

    public struct DynamicParameters
    {
        public ComputeBuffer offsetsBuffer;
        public Vector3 centre;
        public int numPointsPerAxis;
        public int numThreadsPerAxis;
    }

    public struct DynamicParameterInput
    {
        public int seed;
        public int numPointsPerAxis;
        public int threadGroupSize;
        public Vector3 centre;
    }

    public void SetBaseParameters(BaseParameters baseParameters)
    {
        // Points, numPointsPerAxis, boundsSize, Centre, Offset, Spacing, WorldSize
        shader.SetFloat("boundsSize", baseParameters.boundsSize);
        shader.SetFloat("spacing", baseParameters.spacing);
        shader.SetVector("worldSize", baseParameters.worldSize);

        Vector4 offset = new Vector4(0, 0, 0, 0); // FIX
        shader.SetVector("offset", offset);

        shader.SetVector("params", new Vector4(1, 1, 1, 1)); // FIX
    }

    public void SetDynamicParameters(DynamicParameters dynamicParameters)
    {
        shader.SetBuffer(0, "points", pointsBuffer);
        shader.SetBuffer(0, "offsets", dynamicParameters.offsetsBuffer);
        shader.SetVector("centre", dynamicParameters.centre);
        shader.SetInt("numPointsPerAxis", dynamicParameters.numPointsPerAxis);
        shader.SetInt("numThreadsPerAxis", dynamicParameters.numThreadsPerAxis);
    }

    public DynamicParameters GenerateDynamicParameters(DynamicParameterInput dynamicParameterInput)
    {
        var prng = new System.Random(dynamicParameterInput.seed);
        var offsets = new Vector3[numOctaves];
        float offsetRange = 1000;
        for (int i = 0; i < numOctaves; i++) {
            offsets[i] = new Vector3 ((float) prng.NextDouble () * 2 - 1, (float) prng.NextDouble () * 2 - 1, (float) prng.NextDouble () * 2 - 1) * offsetRange;
        }

        var offsetsBuffer = new ComputeBuffer(offsets.Length, sizeof(float) * 3);
        offsetsBuffer.SetData(offsets);

        buffersToRelease.Add(offsetsBuffer);

        int numThreadsPerAxis = Mathf.CeilToInt(dynamicParameterInput.numPointsPerAxis / (float) dynamicParameterInput.threadGroupSize);

        // POINTS BUFFER IS IMPORTANT ; RETURNS THE DENSITIES
        pointsBuffer = new ComputeBuffer(dynamicParameterInput.numPointsPerAxis * dynamicParameterInput.numPointsPerAxis * dynamicParameterInput.numPointsPerAxis, sizeof(float) * 4);

        return new DynamicParameters
        {
            offsetsBuffer = offsetsBuffer,
            centre = dynamicParameterInput.centre,
            numPointsPerAxis = dynamicParameterInput.numPointsPerAxis,
            numThreadsPerAxis = numThreadsPerAxis
        };
    }

    public void Dispatch(int numThreadsPerAxis)
    {
        shader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);
    }
}