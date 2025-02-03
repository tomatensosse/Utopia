using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class DensityInspectorPoint : MonoBehaviour
{
    public TMP_Text text;

    public Vector3Int chunkIndex;
    public Vector3Int pointIndex;
    [ReadOnly] public float density;

    public void SetDensity(float density)
    {
        string densityString = density.ToString("F2");
        text.text = densityString;
    }
}