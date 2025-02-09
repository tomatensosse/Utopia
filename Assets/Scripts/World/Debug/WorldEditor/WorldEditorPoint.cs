using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class WorldEditorPoint : MonoBehaviour
{
    public TMP_Text text;
    public MeshRenderer meshRenderer;

    public Vector3Int chunkIndex;
    public Vector3Int pointIndex;
    [ReadOnly] public float density;

    public void SetDensity(float density)
    {
        string densityString = density.ToString("F2");
        text.text = densityString;
    }

    public void SetColor(Color color)
    {
        Material newMaterial = new Material(meshRenderer.material);
        newMaterial.color = color;

        meshRenderer.material = newMaterial;
    }
}