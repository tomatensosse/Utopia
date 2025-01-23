using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Simple Noise Container", menuName = "Compute/Simple Noise")]
public class SimpleNoiseContainer : CSContainer
{
    [OnInspectorInit]
    private void OnEnable()
    {
        InitializeSharedParameters();
        
        // Add Simple Noise specific parameters
        parameters.Add(new CSParameter("octaves", "Octaves", "Number of noise octaves", CSParameterType.Int, 4));
        parameters.Add(new CSParameter("lacunarity", "Lacunarity", "Frequency multiplier per octave", CSParameterType.Float, 2f));
        parameters.Add(new CSParameter("persistence", "Persistence", "Amplitude multiplier per octave", CSParameterType.Float, 0.5f));
        parameters.Add(new CSParameter("noiseScale", "Noise Scale", "Scale of the noise", CSParameterType.Float, 1f));
        parameters.Add(new CSParameter("noiseWeight", "Noise Weight", "Weight of the noise", CSParameterType.Float, 1f));
        parameters.Add(new CSParameter("floorOffset", "Floor Offset", "Offset of the floor", CSParameterType.Float, 0f));
        parameters.Add(new CSParameter("weightMultiplier", "Weight Multiplier", "Multiplier for weights", CSParameterType.Float, 1f));
        parameters.Add(new CSParameter("closeEdges", "Close Edges", "Whether to close edges", CSParameterType.Bool, false));
        parameters.Add(new CSParameter("hardFloor", "Hard Floor", "Height of hard floor", CSParameterType.Float, -10f));
        parameters.Add(new CSParameter("hardFloorWeight", "Hard Floor Weight", "Weight of hard floor", CSParameterType.Float, 1f));
    }
}