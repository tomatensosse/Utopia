using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class BlendNode : Node
{
    public ComputeShader shader;
    public ComputeBuffer pointsBuffer;

    [Space(10)]
    [LabelText("Node A")]
    [SerializeReference]
    public Node a;

    [Space(10)]
    [LabelText("Node B")]
    [SerializeReference]
    public Node b;

    [Space(10)]
    [LabelText("Blend Weight")]
    [Range(0, 1)]
    public float weight;

    public override ComputeBuffer GenerateDensity(DebugChunk debugChunk)
    {
        ComputeBuffer bufferA = a.GenerateDensity(debugChunk);
        ComputeBuffer bufferB = b.GenerateDensity(debugChunk);

        int numPointsPerAxis = World.Settings.numPointsPerAxis;
        pointsBuffer = new ComputeBuffer(numPointsPerAxis * numPointsPerAxis * numPointsPerAxis, sizeof(float) * 4);

        shader.SetBuffer(0, "points", pointsBuffer);
        shader.SetBuffer(0, "pointsA", bufferA);
        shader.SetBuffer(0, "pointsB", bufferB);
        shader.SetFloat("weight", weight);
        shader.SetInt("numPointsPerAxis", numPointsPerAxis);

        int threadGroupSize = World.Settings.threadGroupSize;

        shader.Dispatch(0, threadGroupSize, threadGroupSize, threadGroupSize);

        bufferA.Release();
        bufferB.Release();

        return pointsBuffer;
    }
}