using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "World/Biome")]
public class Biome : ScriptableObject
{
    [Header("Biome Settings")]
    public string uid;
    public string biomeName;
    public Color biomeColor = Color.white;
    public Material material;

    [SerializeReference]
    public Node root;
}