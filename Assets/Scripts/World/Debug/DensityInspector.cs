using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DensityInspector : MonoBehaviour
{
    public static DensityInspector Instance { get; private set; }
    public Camera inspectorCamera;
    public float positionLerp = 32f;
    public GameObject densityInspectorPointPrefab;

    private static int renderDistance => ChunkGenerator.Instance.inspectorRenderDistance;

    private bool isReady = false;
    private bool isMoving = false;

    private Vector3Int _chunkIndex = Vector3Int.zero;
    private Vector3Int _pointIndex = Vector3Int.zero;
    private Chunk currentChunk;
    private GameObject pointContainer;

    private Vector3Int input;

    private Vector3 offset;

    private Dictionary<Vector3Int, DensityInspectorPoint> _points = new Dictionary<Vector3Int, DensityInspectorPoint>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _points.Clear();

        pointContainer = new GameObject("Density Points");

        offset = (-1) * new Vector3(World.Settings.chunkSize / 2f, World.Settings.chunkSize / 2f, World.Settings.chunkSize / 2f);
    }

    void Update()
    {
        HandleInput();
        HandleMove();
    }

    private void HandleInput()
    {
        input = Vector3Int.zero;

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
    }

    private void HandleMove()
    {
        if (input == Vector3Int.zero || isMoving) // Return if no input
        {
            return;
        }
        if (currentChunk.PointInChunk(_pointIndex + input)) // If point is in chunk move
        {
            _pointIndex += input;
            StartCoroutine(SmoothMove(_pointIndex));
            return;
        }
        if (!currentChunk.PointInChunk(_pointIndex + input))
        {
            if (ChunkGenerator.Chunks.TryGetValue(_chunkIndex + input, out Chunk chunk))
            {
                currentChunk = chunk;
                _chunkIndex += input;

                Vector3Int newPointIndex = -input * (World.Settings.numPointsPerAxis - 1) + _pointIndex;
                _pointIndex = newPointIndex;

                StartCoroutine(SmoothMove(_pointIndex));
                return;
            }
        }
    }

    IEnumerator SmoothMove(Vector3Int toPointIndex)
    {
        isMoving = true;

        Vector3 toPosition = ((Vector3)toPointIndex * World.Settings.pointSpacing) + offset + currentChunk.transform.position;

        while (Vector3.Distance(transform.position, toPosition) > 0.10f)
        {
            transform.position = Vector3.Lerp(transform.position, toPosition, positionLerp * Time.deltaTime);
            yield return null;
        }

        transform.position = toPosition;

        isMoving = false;
    }

    public void SetReady()
    {
        _chunkIndex = Vector3Int.zero;
        _pointIndex = Vector3Int.zero;

        if (ChunkGenerator.Chunks.TryGetValue(_chunkIndex, out Chunk chunk))
        {
            currentChunk = chunk;
        }

        isReady = true;
    }
}