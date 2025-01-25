using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "World/Biome")]
public class Biome : ScriptableObject
{
    public Material material;

    [SerializeReference]
    public Node root;
}

[System.Serializable]
public abstract class Node
{
    public abstract ComputeBuffer GenerateDensity(DebugChunk debugChunk);
}

[System.Serializable]
public class BlendNode : Node
{
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
        return null;
    }
}