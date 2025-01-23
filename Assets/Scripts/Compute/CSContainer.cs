using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Compute Shader Container", menuName = "World/Compute/Shader Container")]
public class CSContainer : SerializedScriptableObject
{
    [Required]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    public ComputeShader shader;

    [PropertyOrder(-1)]
    [InfoBox("$GetShaderInfo")]
    public string kernelName = "Density";

    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "displayName")]
    [PropertySpace(SpaceAfter = 20)]
    public List<CSParameter> parameters = new List<CSParameter>();

    private string GetShaderInfo()
    {
        if (shader == null) return "No shader assigned";
        return $"Shader: {shader.name}\nKernel: {kernelName}";
    }

    protected virtual void InitializeSharedParameters()
    {
        parameters.Add(new CSParameter("numPointsPerAxis", "Points Per Axis", "Number of points per axis", CSParameterType.Int, 32));
        parameters.Add(new CSParameter("boundsSize", "Bounds Size", "Size of the bounds", CSParameterType.Float, 1f));
        parameters.Add(new CSParameter("centre", "Center", "Center position", CSParameterType.Vector3, Vector3.zero));
        parameters.Add(new CSParameter("offset", "Offset", "Position offset", CSParameterType.Vector3, Vector3.zero));
        parameters.Add(new CSParameter("spacing", "Point Spacing", "Space between points", CSParameterType.Float, 1f));
        parameters.Add(new CSParameter("worldSize", "World Size", "Size of the world", CSParameterType.Vector3, Vector3.one));
    }
}
