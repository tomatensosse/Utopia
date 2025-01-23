using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Ridged Noise Container", menuName = "Compute/Ridged Noise")]
public class RidgedNoiseContainer : CSContainer
{
    [OnInspectorInit]
    private void OnEnable()
    {
        InitializeSharedParameters();
        
        // Add Ridged Noise specific parameters
        parameters.Add(new CSParameter("octaves", "Octaves", "Number of noise octaves", CSParameterType.Int, 4));
        parameters.Add(new CSParameter("lacunarity", "Lacunarity", "Frequency multiplier per octave", CSParameterType.Float, 2f));
        parameters.Add(new CSParameter("persistence", "Persistence", "Amplitude multiplier per octave", CSParameterType.Float, 0.5f));
        parameters.Add(new CSParameter("noiseScale", "Noise Scale", "Scale of the noise", CSParameterType.Float, 1f));
        parameters.Add(new CSParameter("ridgeWeight", "Ridge Weight", "Weight of the ridges", CSParameterType.Float, 1f));
    }
}
