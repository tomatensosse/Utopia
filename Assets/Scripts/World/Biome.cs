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