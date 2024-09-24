// https://github.com/tomatensosse/Prototype
// WORLDGENERATOR v1.0 : MapGenerator.cs
// By
// Emre Bora Kaynar
// Arda Gürses

using System.Collections.Generic;
using UnityEngine;

public class BiomePoints
{
    public Biome biome;
    public List<Vector2Int> points = new List<Vector2Int>();
}

public class BiomeOrigin
{
    public Vector2Int origin;
    public int biomeID;
}

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance;
    public UnityEngine.UI.Image worldVisualisationUIImage;
    public int mapSizeInChunks = 256;
    public int islandOffsetFromEdges = 16;
    public int falloffConstantDistance = 14;
    public int falloffAfterConstantDistance = 8;
    public Biome[] biomes;
    public int biomeDistance;
    public Biome beachBiome;
    public int beachStrength = 2;
    public Biome oceanBiome;
    public Biome lakeBiome;

    [SerializeField] float noiseScale = 1f;
    [Range(0.0f, 1.0f)]
    [SerializeField] float noiseTreshold = 0.5f;
    int seed = 0;
    [HideInInspector] private int[,] biomeMap;
    BiomePoints[] biomePointsArray;
    List<BiomeOrigin> biomeOrigins = new List<BiomeOrigin>();

    private void Awake()
    {
        Instance = this;
    }

    public int[,] BiomeMap(int seed)
    {
        Random.InitState(seed);

        biomeMap = new int[mapSizeInChunks, mapSizeInChunks];

        GenerateBiomePoints();

        GenerateBaseMap();

        foreach (Biome biome in biomes)
        {
            GenerateOoze(biome, biome.spawnInBiome);
        }

        CustomDebug.OutputMatrix(biomeMap);

        return biomeMap;
    }

    void GenerateBaseMap()
    {
        // generate a random offset for the perlin noise
        int randomOffset = Random.Range(-10000, 10000);

        // set every cell to default cell with biome index 0
        for (int x = 0; x < mapSizeInChunks; x++)
        {
            for (int y = 0; y < mapSizeInChunks; y++)
            {
                biomeMap[x, y] = 0;

                int index = GetIndexFromBiomePointsArrayFromID(0);
                biomePointsArray[index].points.Add(new Vector2Int(x, y));
            }
        }

        // falloff
        // set the edges of the map with perlin noise to make the square look like an island
        for (int x = 0; x < mapSizeInChunks; x++)
        {
            for (int y = 0; y < mapSizeInChunks; y++)
            {
                float distanceFromEdge = Mathf.Min(Mathf.Min(x, y), Mathf.Min(mapSizeInChunks - x, mapSizeInChunks - y));
                float falloff = 1 - (distanceFromEdge / falloffConstantDistance);
                falloff = Mathf.Clamp01(falloff);
                falloff = Mathf.Pow(falloff, falloffAfterConstantDistance);

                // Check if the pixel is inside the island offset from edges
                if (distanceFromEdge <= islandOffsetFromEdges)
                {
                    // Increase the threshold
                    float increasedThreshold = noiseTreshold + (islandOffsetFromEdges - distanceFromEdge) / islandOffsetFromEdges;
                    if (Mathf.PerlinNoise((x * 0.123f + randomOffset) * noiseScale, (y * 0.123f + randomOffset) * noiseScale) < increasedThreshold * falloff)
                    {
                        biomeMap[x, y] = -1;
                    }
                }
                else
                {
                    if (Mathf.PerlinNoise((x * 0.123f + randomOffset) * noiseScale, (y * 0.123f + randomOffset) * noiseScale) < noiseTreshold * falloff)
                    {
                        biomeMap[x, y] = -1;
                    }
                }
            }
        }

        int[,] pointsToSet = new int[mapSizeInChunks, mapSizeInChunks];

        // store the points that will be set to beach biome in a temporary array
        for (int x = 0; x < mapSizeInChunks; x++)
        {
            for (int y = 0; y < mapSizeInChunks; y++)
            {
                if (biomeMap[x, y] == 0)
                {
                    if (x > 0 && biomeMap[x - 1, y] == -1)
                    {
                        biomeMap[x, y] = -2;
                        for (int i = 1; i <= beachStrength; i++)
                        {
                            if (x + i >= 0 && biomeMap[x + i, y] == 0)
                            {
                                pointsToSet[x + i, y] = -2;
                            }
                        }
                    }
                    if (x < mapSizeInChunks - 1 && biomeMap[x + 1, y] == -1)
                    {
                        biomeMap[x, y] = -2;
                        for (int i = 1; i <= beachStrength; i++)
                        {
                            if (x - i >= 0 && biomeMap[x - i, y] == 0)
                            {
                                pointsToSet[x - i, y] = -2;
                            }
                        }
                    }
                    if (y > 0 && biomeMap[x, y - 1] == -1)
                    {
                        biomeMap[x, y] = -2;
                        for (int i = 1; i <= beachStrength; i++)
                        {
                            if (y + i >= 0 && biomeMap[x, y + i] == 0)
                            {
                                pointsToSet[x, y + i] = -2;
                            }
                        }
                    }
                    if (y < mapSizeInChunks - 1 && biomeMap[x, y + 1] == -1)
                    {
                        biomeMap[x, y] = -2;
                        for (int i = 1; i <= beachStrength; i++)
                        {
                            if (y - i >= 0 && biomeMap[x, y - i] == 0)
                            {
                                pointsToSet[x, y - i] = -2;
                            }
                        }
                    }
                }
            }
        }

        // apply beach biomes to the points
        for (int x = 0; x < mapSizeInChunks; x++)
        {
            for (int y = 0; y < mapSizeInChunks; y++)
            {
                if (pointsToSet[x, y] == -2)
                {
                    biomeMap[x, y] = -2;
                }
            }
        }

        // generate lake biomes on each empty chunk
        for (int x = 0; x < mapSizeInChunks; x++)
        {
            for (int y = 0; y < mapSizeInChunks; y++)
            {
                if (biomeMap[x, y] == -1)
                {
                    biomeMap[x, y] = lakeBiome.specialBiomeID;

                }
            }
        }
        FloodFill(0, 0, -3, -4);

    }
    void FloodFill(int x, int y, int targetValue, int newValue)
    {
        // S�n�rlar� ve hedef de�eri kontrol et
        if (x < 0 || x >= mapSizeInChunks || y < 0 || y >= mapSizeInChunks || biomeMap[x, y] != targetValue)
            return;

        // Hedef de�eri yeni de�er ile de�i�tir
        
        biomeMap[x, y] = newValue;
        
        

        // 4 y�nl� biti�ik olan elemanlar i�in yeniden �a��r
        FloodFill(x + 1, y, targetValue, newValue); // Sa�
        FloodFill(x - 1, y, targetValue, newValue); // Sol
        FloodFill(x, y + 1, targetValue, newValue); // A�a��
        FloodFill(x, y - 1, targetValue, newValue); // Yukar�
    }

    void GenerateBiomePoints()
    {
        // initialise the biomePointsArray
        biomePointsArray = new BiomePoints[biomes.Length + 1];

        // special biomes first

        //beach biome
        BiomePoints beachBiomePoints = new BiomePoints();
        beachBiomePoints.biome = beachBiome;
        
        // regular biomes
        for (int i = 0; i < biomes.Length; i++)
        {
            BiomePoints biomePoints = new BiomePoints();
            biomePoints.biome = biomes[i];
            biomePointsArray[i] = biomePoints;
        }
    }
    
    void OozeGeneration(int[,] matrix, int startRow, int startCol, int maxSpread, int biomeID)
    {
        int counter = maxSpread;
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        Queue<(int, int)> toVisit = new Queue<(int, int)>();
        HashSet<(int, int)> visited = new HashSet<(int, int)>();

        toVisit.Enqueue((startRow, startCol));

        while (toVisit.Count > 0 && visited.Count < maxSpread && counter >= 0)
        {
            counter--;
            var current = toVisit.Dequeue();
            if (visited.Contains(current))
            {
                continue;
            }

            int r = current.Item1;
            int c = current.Item2;
            Debug.Log(matrix[r, c]);
            //dont touch works somehow
            if (matrix[r, c] == 0 || matrix[r, c] == biomeID)
            {
                Debug.Log("Passed");
                matrix[r, c] = biomeID;
                visited.Add(current);
                var neighbors = new List<(int, int)>
                {
                    (r - 1, c), (r + 1, c), (r, c - 1), (r, c + 1)
                };


                foreach (var neighbor in neighbors)
                {
                    int nr = neighbor.Item1;
                    int nc = neighbor.Item2;
                    if (nr >= 0 && nr < rows && nc >= 0 && nc < cols && !visited.Contains((nr, nc)))
                    {
                        if (UnityEngine.Random.value < 0.75f)
                        {
                            toVisit.Enqueue((nr, nc));
                        }

                    }
                }
            }
            else
            {
                
            }
            


            // Mark the cell as visited
            

            // Get neighbors (up, down, left, right)
            
        }
    }
    void GenerateOoze(Biome biome, Biome spawnInBiome)
    {
        if (biome.isDefaultBiome)
        {
            return;
        }
        
        List<Vector2Int> samplePoints = GetBiomePoints(spawnInBiome);

        for (int i = 0; i < biome.maxInstances; i++)
        {
            int chunksLeftTillMax = biome.maxChunksInInstance;
            bool a = true;
            int index = Random.Range(0, samplePoints.Count);    
            Vector2Int point = samplePoints[index];
        
            int pointVal = biomeMap[point.x, point.y];

            foreach (BiomeOrigin biomeOrigin in biomeOrigins)
            {
                if (Vector2.Distance(point, biomeOrigin.origin) < biomeDistance)
                {
                    a = false;
                }
            }


            if (pointVal == spawnInBiome.biomeID && a)
            {
                // generates single point to spread around from
                biomeMap[point.x, point.y] = biome.biomeID;
                Debug.Log("Successfully generated " + biome.biomeName + " at " + point.x + ", " + point.y);

                biomePointsArray[GetIndexFromBiomePointsArrayFromID(biome.biomeID)].points.Add(point);
                biomePointsArray[GetIndexFromBiomePointsArrayFromID(spawnInBiome.biomeID)].points.Remove(point);
                // SPREAD ! GO GURSES !
                BiomeOrigin newBiomeOrigin = new BiomeOrigin();
                newBiomeOrigin.biomeID = biome.biomeID;
                newBiomeOrigin.origin = point;

                biomeOrigins.Add(newBiomeOrigin);

                OozeGeneration(biomeMap, point.x, point.y, biome.maxChunksInInstance, biome.biomeID);
            }
            else
            {
                // if the point is not in the spawnInBiome, try again
                i--;
            }
        }
    }

    List<Vector2Int> GetBiomePoints(Biome biome)
    {
        foreach (BiomePoints biomePoints in biomePointsArray)
        {
            if (biomePoints.biome == biome)
            {
                Debug.Log("Successfully found biome in biomePointsArray.");
                return biomePoints.points;
            }
        }

        Debug.LogError("Biome not found in biomePointsArray.");
        return null;
    }

    int GetIndexFromBiomePointsArrayFromID(int biomeID)
    {
        for (int i = 0; i < biomePointsArray.Length; i++)
        {
            if (biomePointsArray[i].biome.biomeID == biomeID)
            {
                return i;
            }
        }

        return -1;
    }

    public int GetBiomeIndex(int x, int y)
    {
        return biomeMap[x, y];
    }

    public Biome GetBiome(Vector2Int point)
    {
        int x = point.x;
        int y = point.y;

        if (biomeMap[x, y] == -1)
        {
            Debug.Log("EMPTY CELL");
            return null;
        }
        else if (biomeMap[x, y] == -2)
        {
            return beachBiome;
        }
        else if (biomeMap[x, y] == -3)
        {
            return lakeBiome;
        }
        else if (biomeMap[x, y] == -4)
        {
            return oceanBiome;
        }
        else
        {
            return biomes[GetIndexFromBiomePointsArrayFromID(biomeMap[x, y])];
        }
    }
}