using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class CSWrapper
{
    [Required]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    public CSContainer container;

    [DictionaryDrawerSettings(KeyLabel = "Parameter", ValueLabel = "Value")]
    [ShowInInspector]
    private Dictionary<string, object> parameterValues = new Dictionary<string, object>();

    private int kernelIndex;
    
    [Button(ButtonSizes.Medium)]
    [PropertyOrder(-1)]
    private void RefreshParameters()
    {
        if (container != null)
        {
            InitializeParameters();
            kernelIndex = container.shader.FindKernel(container.kernelName);
        }
    }
    
    private void OnEnable()
    {
        RefreshParameters();
    }
    
    private void InitializeParameters()
    {
        parameterValues.Clear();
        foreach (var param in container.parameters)
        {
            parameterValues[param.uid] = param.defaultValue;
        }
    }
    
    public void SetParameter(string uid, object value)
    {
        if (parameterValues.ContainsKey(uid))
        {
            parameterValues[uid] = value;
        }
    }
    
    public ComputeBuffer DispatchShader(ComputeBuffer pointsBuffer, Vector3Int threadGroups)
    {
        if (container == null) return null;
        
        // Set buffer
        container.shader.SetBuffer(kernelIndex, "points", pointsBuffer);
        
        // Set all parameters
        foreach (var param in parameterValues)
        {
            var parameter = container.parameters.Find(p => p.uid == param.Key);
            
            switch (parameter.type)
            {
                case CSParameterType.Int:
                    container.shader.SetInt(param.Key, (int)param.Value);
                    break;
                case CSParameterType.Float:
                    container.shader.SetFloat(param.Key, (float)param.Value);
                    break;
                case CSParameterType.Vector2:
                    container.shader.SetVector(param.Key, (Vector2)param.Value);
                    break;
                case CSParameterType.Vector3:
                    container.shader.SetVector(param.Key, (Vector3)param.Value);
                    break;
                case CSParameterType.Vector4:
                    container.shader.SetVector(param.Key, (Vector4)param.Value);
                    break;
                case CSParameterType.Bool:
                    container.shader.SetBool(param.Key, (bool)param.Value);
                    break;
            }
        }
        
        // Dispatch
        container.shader.Dispatch(kernelIndex, threadGroups.x, threadGroups.y, threadGroups.z);
        
        return pointsBuffer;
    }
}