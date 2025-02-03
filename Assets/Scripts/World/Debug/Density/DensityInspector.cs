using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class DensityInspector : MonoBehaviour
{
    public static DensityInspector Instance { get; private set; }
    public Camera inspectorCamera;
    public float positionLerp = 32f;
    public GameObject densityInspectorPointPrefab;

    private bool isReady = false;
    private bool isMoving = false;

    private Vector3Int _chunkIndex = Vector3Int.zero;
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

        float offsetint = (int)World.Settings.chunkSize / 2;
        offset = new Vector3(offsetint, offsetint, offsetint) * (-1);
    }

    void Update()
    {
        HandleInput();
        HandleMove();
    }

    private void HandleInput()
    {
        input = Vector3Int.zero;

        if (isMoving || !isReady)
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
    }

    private void HandleMove()
    {
        if (isMoving || !isReady)
        {
            return;
        }

        Vector3Int newChunkIndex = _chunkIndex + input;

        if (newChunkIndex != _chunkIndex)
        {
            if (ChunkGenerator.Chunks.TryGetValue(newChunkIndex, out Chunk chunk))
            {
                currentChunk = chunk;
                _chunkIndex = newChunkIndex;
                StartCoroutine(SmoothMove());

                GeneratePoints();
            }
        }
    }

    private void GeneratePoints()
    {
        foreach (DensityInspectorPoint point in _points)
        {
            Destroy(point.gameObject);
        }

        _points.Clear();

        if (currentChunk == null)
        {
            Debug.Log("No chunk found for this index");
            return;
        }

        ComputeBuffer cloneBuffer;

        if (currentChunk.isBlended)
        {
            cloneBuffer = currentChunk.BlendedBuffer;
        }
        else
        {
            cloneBuffer = currentChunk.DensityBuffer;
        }

        if (cloneBuffer == null)
        {
            Debug.Log("No density buffer found for this chunk");
            return;
        }

        int numPointsPerAxis = World.Settings.numPointsPerAxis;;
        int totalPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
        Vector4[] densityPoints = new Vector4[totalPoints];
        cloneBuffer.GetData(densityPoints);

        for (int z = 0; z < numPointsPerAxis; z++)
        {
            for (int y = 0; y < numPointsPerAxis; y++)
            {
                for (int x = 0; x < numPointsPerAxis; x++)
                {
                    GameObject point = Instantiate(densityInspectorPointPrefab, pointContainer.transform);
                    // Use pointSpacing to ensure correct positioning
                    Vector3 positionOfPoint = new Vector3(x, y, z) * World.Settings.pointSpacing + 
                                            offset + 
                                            (World.Settings.chunkSize * _chunkIndex);
                    point.transform.position = positionOfPoint;

                    int index = z * numPointsPerAxis * numPointsPerAxis + 
                            y * numPointsPerAxis + 
                            x;
                    
                    float density = densityPoints[index].w;
                    point.GetComponent<DensityInspectorPoint>().SetDensity(density);
                    _points.Add(point.GetComponent<DensityInspectorPoint>());
                }
            }
        }

        Debug.Log("Points generated");
    }

    IEnumerator SmoothMove()
    {
        isMoving = true;

        Vector3 toPosition = _chunkIndex * World.Settings.chunkSize;

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

        if (ChunkGenerator.Chunks.TryGetValue(_chunkIndex, out Chunk chunk))
        {
            currentChunk = chunk;
        }

        HandleMove();

        isReady = true;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.15f);

        if (currentChunk.isBlended)
        {
            Gizmos.color = new Color(0.5f, 0f, 0.5f, 0.15f);
        }

        Gizmos.DrawCube(transform.position, new Vector3(World.Settings.chunkSize + 1, World.Settings.chunkSize + 1, World.Settings.chunkSize + 1));
    }
}