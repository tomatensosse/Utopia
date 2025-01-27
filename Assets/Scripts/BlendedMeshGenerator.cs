using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendedMeshGenerator : MonoBehaviour
{
    public static BlendedMeshGenerator Instance { get; private set; }
    public static bool Ready => Instance._isReady;

    private bool _isReady = false;

    public ComputeShader blendedMarchingCubesShader;

    [Range(0, 1)]
    public float blendStrength = 1f;

    private ComputeBuffer pointsBuffer;
    private ComputeBuffer triangleBuffer;
    private ComputeBuffer triCountBuffer;
    private ComputeBuffer neighborVerticesBuffer;
    private ComputeBuffer neighborVertexCountsBuffer;
    private ComputeBuffer neighborOffsetsBuffer;
    private ComputeBuffer hasNeighborBuffer;

    private int numPoints => World.Settings.numPoints;
    private int maxTriangleCount => World.Settings.maxTriangleCount;
    private int numThreadsPerAxis => World.Settings.numThreadsPerAxis;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        CreateBuffers();
    }

    void OnDestroy()
    {
        ReleaseBuffers();
    }

    public Mesh GenerateBlendedMesh(ComputeBuffer pointsBuffer, float isoLevel, List<ComputeBuffer> neighborBuffers)
    {
        if (neighborBuffers == null)
        {
            neighborBuffers = new List<ComputeBuffer>();
            Debug.LogWarning("Neighbor buffers list is null. Initializing an empty list.");
        }

        foreach (var buffer in neighborBuffers)
        {
            if (buffer == null)
            {
                // Log warning and continue with non-null buffers
                Debug.LogWarning("An element in neighbor buffers is null.");
            }
        }

        // Set up buffer for neighboring vertices and related data
        int neighborCount = neighborBuffers.Count;
        if (neighborCount > 0)
        {
            CreateNeighborBuffers(neighborBuffers);
        }

        triangleBuffer.SetCounterValue(0);
        blendedMarchingCubesShader.SetBuffer(0, "points", pointsBuffer);
        blendedMarchingCubesShader.SetBuffer(0, "triangles", triangleBuffer);
        blendedMarchingCubesShader.SetBuffer(0, "neighborVertices", neighborVerticesBuffer);
        blendedMarchingCubesShader.SetBuffer(0, "neighborVertexCounts", neighborVertexCountsBuffer);
        blendedMarchingCubesShader.SetBuffer(0, "neighborOffsets", neighborOffsetsBuffer);
        blendedMarchingCubesShader.SetBuffer(0, "hasNeighbor", hasNeighborBuffer);

        blendedMarchingCubesShader.SetInt("numPointsPerAxis", World.Settings.numPointsPerAxis);
        blendedMarchingCubesShader.SetFloat("isoLevel", isoLevel);
        blendedMarchingCubesShader.SetFloat("blendStrength", blendStrength);
        blendedMarchingCubesShader.SetFloat("chunkSize", World.Settings.chunkSize);

        blendedMarchingCubesShader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

        // Get number of triangles in the triangle buffer
        ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);
        int[] triCountArray = { 0 };
        triCountBuffer.GetData(triCountArray);
        int numTris = triCountArray[0];

        // Get triangle data from shader
        Triangle[] tris = new Triangle[numTris];
        triangleBuffer.GetData(tris, 0, 0, numTris);

        Mesh mesh = new Mesh();

        var vertices = new Vector3[numTris * 3];
        var meshTriangles = new int[numTris * 3];

        for (int i = 0; i < numTris; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                meshTriangles[i * 3 + j] = i * 3 + j;
                vertices[i * 3 + j] = tris[i][j];
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = meshTriangles;

        mesh.RecalculateNormals();

        // Release neighbor buffers
        ReleaseNeighborBuffers();

        return mesh;
    }

    private void CreateBuffers()
    {
        if (!Application.isPlaying || pointsBuffer == null || numPoints != pointsBuffer.count)
        {
            if (Application.isPlaying)
            {
                ReleaseBuffers();
            }
            triangleBuffer = new ComputeBuffer(maxTriangleCount, sizeof(float) * 3 * 3, ComputeBufferType.Append);
            pointsBuffer = new ComputeBuffer(numPoints, sizeof(float) * 4);
            triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);

            Debug.Log("Created buffers | isReady!");
            _isReady = true;
        }
    }

    private void ReleaseBuffers()
    {
        if (triangleBuffer != null)
        {
            triangleBuffer.Release();
        }
        if (pointsBuffer != null)
        {
            pointsBuffer.Release();
        }
        if (triCountBuffer != null)
        {
            triCountBuffer.Release();
        }
    }

    private void CreateNeighborBuffers(List<ComputeBuffer> neighborBuffers)
    {
        int totalNeighbors = neighborBuffers.Count;
        List<int> neighborVertexCounts = new List<int>();
        List<Vertex> neighborVertices = new List<Vertex>();
        List<int> neighborOffsets = new List<int>();
        List<int> hasNeighbor = new List<int>();

        int offset = 0;
        foreach (var buffer in neighborBuffers)
        {
            // Skip null buffers
            if (buffer == null)
            {
                hasNeighbor.Add(0);
                neighborOffsets.Add(0);
                neighborVertexCounts.Add(0);
                continue;
            }

            try
            {
                int count = buffer.count;  // Wrap this in try-catch
                neighborOffsets.Add(offset);
                neighborVertexCounts.Add(count);
                hasNeighbor.Add(1);

                Vertex[] vertices = new Vertex[count];
                buffer.GetData(vertices);
                neighborVertices.AddRange(vertices);

                offset += count;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Error processing neighbor buffer: {e.Message}");
                hasNeighbor.Add(0);
                neighborOffsets.Add(0);
                neighborVertexCounts.Add(0);
            }
        }

        // Create minimum size buffers if no valid data
        if (neighborVertices.Count == 0)
        {
            neighborVertices.Add(new Vertex());
        }

        // Create buffers even if they might be empty
        neighborVerticesBuffer = new ComputeBuffer(Mathf.Max(1, neighborVertices.Count), sizeof(float) * 4); // 16 bytes stride
        if (neighborVertices.Count > 0)
        {
            neighborVerticesBuffer.SetData(neighborVertices.ToArray());
        }
        else
        {
            neighborVerticesBuffer.SetData(new Vertex[1]);
        }

        neighborVertexCountsBuffer = new ComputeBuffer(Mathf.Max(1, neighborVertexCounts.Count), sizeof(int));
        if (neighborVertexCounts.Count > 0)
        {
            neighborVertexCountsBuffer.SetData(neighborVertexCounts.ToArray());
        }
        else
        {
            neighborVertexCountsBuffer.SetData(new int[1]);
        }

        neighborOffsetsBuffer = new ComputeBuffer(Mathf.Max(1, neighborOffsets.Count), sizeof(int));
        if (neighborOffsets.Count > 0)
        {
            neighborOffsetsBuffer.SetData(neighborOffsets.ToArray());
        }
        else
        {
            neighborOffsetsBuffer.SetData(new int[1]);
        }

        hasNeighborBuffer = new ComputeBuffer(Mathf.Max(1, hasNeighbor.Count), sizeof(int));
        if (hasNeighbor.Count > 0)
        {
            hasNeighborBuffer.SetData(hasNeighbor.ToArray());
        }
        else
        {
            hasNeighborBuffer.SetData(new int[1]);
        }
    }

    private void ReleaseNeighborBuffers()
    {
        if (neighborVerticesBuffer != null) neighborVerticesBuffer.Release();
        if (neighborVertexCountsBuffer != null) neighborVertexCountsBuffer.Release();
        if (neighborOffsetsBuffer != null) neighborOffsetsBuffer.Release();
        if (hasNeighborBuffer != null) hasNeighborBuffer.Release();
    }

    struct Triangle
    {
#pragma warning disable 649 // disable unassigned variable warning
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public Vector3 this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return a;
                    case 1:
                        return b;
                    default:
                        return c;
                }
            }
        }
    }

    struct Vertex
    {
        public Vector3 position;
        public float padding; // Add padding to match 16-byte stride
    }
}