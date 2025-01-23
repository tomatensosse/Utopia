using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "World/Biome")]
public class Biome : SerializedScriptableObject
{
    [TableList(ShowIndexLabels = true)]
    public CSWrapper[] noiseGenerators;
    
    [Button(ButtonSizes.Large)]
    public ComputeBuffer GenerateDensity(Vector3Int resolution)
    {
        int numPoints = resolution.x * resolution.y * resolution.z;
        ComputeBuffer pointsBuffer = new ComputeBuffer(numPoints, sizeof(float) * 4);
        
        Vector3Int threadGroups = new Vector3Int(
            Mathf.CeilToInt(resolution.x / 8f),
            Mathf.CeilToInt(resolution.y / 8f),
            Mathf.CeilToInt(resolution.z / 8f)
        );
        
        foreach (var generator in noiseGenerators)
        {
            pointsBuffer = generator.DispatchShader(pointsBuffer, threadGroups);
        }
        
        return pointsBuffer;
    }
    
    private void OnDestroy()
    {
        // Make sure to release any compute buffers when done
    }
}