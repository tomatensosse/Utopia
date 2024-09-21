// https://github.com/tomatensosse/Prototype
// WORLDGENERATOR v1.0 : Biome.cs
// By
// Emre Bora Kaynar
// Arda Gürses

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Biome", menuName = "Terrain Generation/Biome")]
public class Biome : ScriptableObject
{
    public string biomeName;
    public Color biomeColor;

    [Header("Generation")]
    public bool regularBiome = true;
    [ConditionalHide(nameof(regularBiome), false)]
    [Tooltip("If this is a special biome, set the id in the minus range. Ex: Empty biome is -1, beach biome is -2...")]
    public int specialBiomeID = -1;
    [ConditionalHide(nameof(regularBiome), true)]
    public bool isDefaultBiome = false;
    [ConditionalHide(nameof(regularBiome), true)]
    [Tooltip("If this is a regular biome, set the id in the positive range. Ex: Default biome is 0, Forest biome is 1, Desert biome is 2...")]
    public int biomeID = 1;
    [ConditionalHide(nameof(regularBiome), true)]
    public int maxInstances = 1;
    [ConditionalHide(nameof(regularBiome), true)]
    public int maxChunksInInstance = 16;
    [ConditionalHide(nameof(regularBiome), true)]
    public int fullDecayTimeInSteps = 5;
    public Biome spawnInBiome;

    [Header("Terrain")]
    
    public GameObject[] heightGenerators;

    [Header("Rendering")]
    public Material material;
    [Header("Spawning")]
    public List<Spawnable> spawnables;
}