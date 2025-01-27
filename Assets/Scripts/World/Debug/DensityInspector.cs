using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private List<DensityInspectorPoint> _points = new List<DensityInspectorPoint>();

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
            HandleGeneration();
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
                HandleGeneration();
                return;
            }
        }
    }

    private void HandleGeneration()
    {
        List<DensityInspectorPoint> pointsToRemove = new List<DensityInspectorPoint>();
        
        foreach (DensityInspectorPoint point in _points)
        {
            pointsToRemove.Add(point);
        }

        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int y = -renderDistance; y <= renderDistance; y++)
            {
                for (int z = -renderDistance; z <= renderDistance; z++)
                {
                    Vector3Int point = _pointIndex + new Vector3Int(x, y, z);
                    
                    RequestDensity(point);

                    pointsToRemove.RemoveAll(p => p.pointIndex == point && p.chunkIndex == currentChunk.chunkPosition);
                }
            }
        }

        foreach (DensityInspectorPoint point in pointsToRemove)
        {
            _points.Remove(point);
            Destroy(point.gameObject);
        }
    }

    private void RequestDensity(Vector3Int point)
    {
        if (ChunkGenerator.Chunks.TryGetValue(_chunkIndex, out Chunk chunk))
        {
            if (chunk.PointInChunk(point))
            {
                if (_points.Exists(p => p.pointIndex == point && p.chunkIndex == chunk.chunkPosition))
                {
                    return;
                }

                float density = chunk.GetDensity(point);

                GeneratePoint(point, currentChunk.chunkPosition, density);
            }

            if (!chunk.PointInChunk(point))
            {
                if (ChunkGenerator.Chunks.TryGetValue(_chunkIndex + new Vector3Int(Mathf.FloorToInt(point.x / World.Settings.numPointsPerAxis), Mathf.FloorToInt(point.y / World.Settings.numPointsPerAxis), Mathf.FloorToInt(point.z / World.Settings.numPointsPerAxis)), out Chunk newChunk))
                {
                    Vector3Int newPoint = new Vector3Int(point.x % World.Settings.numPointsPerAxis, point.y % World.Settings.numPointsPerAxis, point.z % World.Settings.numPointsPerAxis);

                    if (!newChunk.PointInChunk(newPoint))
                    {
                        return;
                    }

                    if (_points.Exists(p => p.pointIndex == newPoint && p.chunkIndex == newChunk.chunkPosition))
                    {
                        return;
                    }

                    float density = newChunk.GetDensity(newPoint);

                    GeneratePoint(point, newChunk.chunkPosition, density);
                }
            }
        }
    }

    private void GeneratePoint(Vector3Int point, Vector3Int chunk, float density)
    {
        GameObject pointObject = Instantiate(densityInspectorPointPrefab, pointContainer.transform);
        pointObject.transform.position = ((Vector3)point * World.Settings.pointSpacing) + offset + currentChunk.transform.position;

        DensityInspectorPoint densityInspectorPoint = pointObject.GetComponent<DensityInspectorPoint>();
        densityInspectorPoint.chunkIndex = chunk;
        densityInspectorPoint.pointIndex = point;
        densityInspectorPoint.SetDensity(density);

        _points.Add(densityInspectorPoint);
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