using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public static MeshGenerator Instance { get; private set; }
    public static bool Ready => Instance._isReady;

    private bool _isReady = false;

    public ComputeShader marchingCubesShader;

    private ComputeBuffer pointsBuffer;
    private ComputeBuffer triangleBuffer;
    private ComputeBuffer triCountBuffer;

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
        if (triangleBuffer != null)
        {
            triangleBuffer.Release();
            triangleBuffer = null;
        }
        if (pointsBuffer != null)
        {
            pointsBuffer.Release();
            pointsBuffer = null;
        }
        if (triCountBuffer != null)
        {
            triCountBuffer.Release();
            triCountBuffer = null;
        }
    }

    public Mesh GenerateMesh(ComputeBuffer pointsBuffer, float isoLevel)
    {
        triangleBuffer.SetCounterValue(0);
        marchingCubesShader.SetBuffer(0, "points", pointsBuffer);
        marchingCubesShader.SetBuffer(0, "triangles", triangleBuffer);
        marchingCubesShader.SetInt("numPointsPerAxis", World.Settings.numPointsPerAxis);
        marchingCubesShader.SetFloat("isoLevel", isoLevel);

        marchingCubesShader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

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

        for (int i = 0; i < numTris; i++) {
            for (int j = 0; j < 3; j++) {
                meshTriangles[i * 3 + j] = i * 3 + j;
                vertices[i * 3 + j] = tris[i][j];
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = meshTriangles;

        mesh.RecalculateNormals();

        return mesh;
    }

    private void CreateBuffers()
    {
        if (!Application.isPlaying || pointsBuffer == null || numPoints != pointsBuffer.count) {
            if (Application.isPlaying) {
                ReleaseBuffers ();
            }
            triangleBuffer = new ComputeBuffer (maxTriangleCount, sizeof (float) * 3 * 3, ComputeBufferType.Append);
            pointsBuffer = new ComputeBuffer (numPoints, sizeof (float) * 4);
            triCountBuffer = new ComputeBuffer (1, sizeof (int), ComputeBufferType.Raw);

            Debug.Log("Created buffers | isReady!");
            _isReady = true;
        }
    }

    private void ReleaseBuffers()
    {
        if (triangleBuffer != null)
        {
            triangleBuffer.Release();
            pointsBuffer.Release();
            triCountBuffer.Release();
        }
    }

    struct Triangle {
#pragma warning disable 649 // disable unassigned variable warning

        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public Vector3 this [int i] {
            get {
                switch (i) {
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
}