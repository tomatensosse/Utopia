using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEditor : MonoBehaviour
{
    public static WorldEditor Instance { get; private set; }
    
    public List<Biome> biomes = new List<Biome>();
    public Color chunkInspectColor = Color.white;
    public Color emptyInspectColor = Color.white;

    public Vector3Int chunkPosition;
    public Chunk currentChunk;
    public Biome addingBiome;
    private Vector3Int input;

    private List<DensityInspectorPoint> densityPoints = new List<DensityInspectorPoint>();

    private const float lerp = 32f;
    private float gizmoScale;
    private Color gizmoColor;
    private bool isWorking => IsWorking();
    private bool isMoving;
    private bool isCreating;
    private bool isChangingGizmos;
    private bool isReady;

    private float offset;
    private Vector3 offsetVector;
    private GameObject densityPointsContainer;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        StartCoroutine(WaitUntilWorldReady());
    }

    private IEnumerator WaitUntilWorldReady()
    {
        yield return new WaitUntil(() => World.Instance != null);
        yield return new WaitUntil(() => World.Ready);

        Initialize();
        HandleMove(true);
        CycleBiome();
    }

    void Update()
    {
        if (!isReady)
        {
            return;
        }

        HandleInput();
        HandleMove();
    }

    private void Initialize()
    {
        offset = (float)World.Settings.chunkSize / 2 * -1;
        offsetVector = new Vector3(offset, offset, offset);

        densityPointsContainer = new GameObject("Density Points Container");

        isReady = true;
    }

    private void HandleInput()
    {
        input = Vector3Int.zero;

        if (isWorking || !isReady)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            input.z = 1;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            input.z = -1;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            input.x = -1;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            input.x = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            input.y = -1;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            input.y = 1;
        }

        if (Input.GetKeyDown(KeyCode.K) && input == Vector3Int.zero)
        {
            if (ChunkGenerator.Chunks.TryGetValue(chunkPosition, out Chunk chunk))
            {
                AssignBiome();
                return;
            }

            GenerateNewChunk();
        }

        if (Input.GetKeyDown(KeyCode.L) && input == Vector3Int.zero)
        {
            CycleBiome();
        }

        if (Input.GetKeyDown(KeyCode.J) && input == Vector3Int.zero)
        {
            GenerateDensity();
        }

        if (Input.GetKeyDown(KeyCode.M) && input == Vector3Int.zero)
        {
            GenerateMesh();
        }
    }

    private void HandleMove(bool force = false)
    {
        if ((isWorking || !isReady) && !force)
        {
            return;
        }

        if (input == Vector3Int.zero && !force)
        {
            return;
        }

        Vector3Int newChunkPosition = chunkPosition + input;

        if (newChunkPosition != chunkPosition || force)
        {
            chunkPosition = newChunkPosition;
            StartCoroutine(SmoothMove());

            if (ChunkGenerator.Chunks.TryGetValue(newChunkPosition, out Chunk chunk))
            {
                currentChunk = chunk;
                StartCoroutine(SmoothGizmos(World.Settings.chunkSize, chunkInspectColor));
            }
            else
            {
                currentChunk = null;
                StartCoroutine(SmoothGizmos(World.Settings.chunkSize / 2, emptyInspectColor));
            }

            WorldEditorGUI.Instance.PositionUpdated();
        }
    }

    private void GenerateNewChunk()
    {
        if (isCreating || !isReady)
        {
            return;
        }

        isCreating = true;

        ChunkGenerator.Instance.GenerateChunk(chunkPosition);

        HandleMove(true);

        isCreating = false;
    }

    private void AssignBiome()
    {
        if (currentChunk == null)
        {
            Debug.LogWarning("No current chunk to assign biome to!");
            return;
        }

        if (addingBiome == null)
        {
            Debug.LogWarning("No biome to assign to current chunk!");
            return;
        }

        currentChunk.biome = addingBiome;
    }

    private void CycleBiome()
    {
        if (addingBiome == null)
        {
            addingBiome = biomes[0];
        }
        else
        {
            int index = biomes.IndexOf(addingBiome);
            index++;

            if (index >= biomes.Count)
            {
                index = 0;
            }

            addingBiome = biomes[index];
        }
    }

    private void GenerateDensity()
    {
        if (currentChunk == null)
        {
            Debug.LogWarning("No current chunk to generate density points for!");
            return;
        }

        if (currentChunk.biome == null)
        {
            Debug.LogWarning("Current chunk has no biome to generate density points for!");
            return;
        }

        if (currentChunk.isDensityGenerated)
        {
            Debug.LogWarning("Current chunk already has density points generated!");
            return;
        }

        currentChunk.GenerateDensity();
    }

    private void GenerateMesh()
    {
        if (currentChunk == null)
        {
            Debug.LogWarning("No current chunk to generate mesh for!");
            return;
        }

        if (currentChunk.biome == null)
        {
            Debug.LogWarning("Current chunk has no biome to generate mesh for!");
            return;
        }

        currentChunk.GenerateMesh(false); // Releasing buffers after complete !!!
    }

    private IEnumerator SmoothMove()
    {
        isMoving = true;
        
        Vector3 toPosition = chunkPosition * World.Settings.chunkSize;

        while (Vector3.Distance(transform.position, toPosition) > 0.10f)
        {
            transform.position = Vector3.Lerp(transform.position, toPosition, lerp * Time.deltaTime);

            yield return null;
        }

        transform.position = toPosition;

        isMoving = false;
    }

    private IEnumerator SmoothGizmos(float toScale, Color toColor)
    {
        isChangingGizmos = true;

        while (Mathf.Abs(gizmoScale - toScale) > 0.10f && gizmoColor != toColor && isWorking)
        {
            gizmoScale = Mathf.Lerp(gizmoScale, toScale, lerp * Time.deltaTime);
            gizmoColor = Color.Lerp(gizmoColor, toColor, lerp * Time.deltaTime);
            yield return null;
        }

        gizmoScale = toScale;
        gizmoColor = toColor;

        isChangingGizmos = false;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position, Vector3.one * gizmoScale);
    }

    private bool IsWorking()
    {
        return isMoving || isCreating || isChangingGizmos;
    }
}