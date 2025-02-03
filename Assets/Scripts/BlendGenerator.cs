using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendGenerator : MonoBehaviour
{
    public static BlendGenerator Instance { get; private set; }
    
    [SerializeField] private ComputeShader blendShader;
    [SerializeField] private int blendDistance = 8;
    [SerializeField] private int falloffBegin = 6;

    void Awake() => Instance = this;

    public ComputeBuffer BlendWithNeighbor(ComputeBuffer centerDensities, ComputeBuffer neighborDensities, Vector3Int neighborRelative)
    {
        int numPointsPerAxis = World.Settings.numPointsPerAxis;
        int totalPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;

        // Create output buffer with copy of center densities
        ComputeBuffer outputBuffer = new ComputeBuffer(totalPoints, sizeof(float) * 4);
        Vector4[] centerData = new Vector4[totalPoints];
        centerDensities.GetData(centerData);
        outputBuffer.SetData(centerData);

        Debug.Log("Blending densities...");

        try
        {
            int kernelIndex = blendShader.FindKernel("BlendDensities");

            // Set shader parameters
            blendShader.SetBuffer(kernelIndex, "points", outputBuffer);
            blendShader.SetBuffer(kernelIndex, "neighborPoints", neighborDensities);
            blendShader.SetVector("neighborRelativity", new Vector4(neighborRelative.x, neighborRelative.y, neighborRelative.z));
            blendShader.SetInt("numPointsPerAxis", numPointsPerAxis);
            blendShader.SetInt("blendDistance", blendDistance);
            blendShader.SetInt("falloffBegin", falloffBegin);

            // Dispatch
            int threadGroups = Mathf.CeilToInt(numPointsPerAxis / 8f);
            blendShader.Dispatch(kernelIndex, threadGroups, threadGroups, threadGroups);

            return outputBuffer;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in density blending: {e}");
            outputBuffer?.Release();
            return null;
        }
    }

    private void OnValidate()
    {
        // Ensure falloffBegin is less than blendDistance
        if (falloffBegin >= blendDistance)
        {
            falloffBegin = blendDistance - 1;
            Debug.LogWarning("Falloff Begin must be less than Blend Distance. Adjusting value.");
        }
    }
}