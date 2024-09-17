using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    public Color chunkBoundsColor;
    public Biome chunkBiome;
    public int chunkSizeHorizontal;
    public int chunkSizeVertical;

    public Vector2Int chunkPosition;

    [HideInInspector] public bool setUp = false;
    [HideInInspector] public bool meshIsSet = false;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    public bool leftEdgeInterpolated = false;
    public bool rightEdgeInterpolated = false;
    public bool frontEdgeInterpolated = false;
    public bool backEdgeInterpolated = false;

    public bool frontLeftEdgeInterpolated = false;
    public bool frontRightEdgeInterpolated = false;
    public bool backLeftEdgeInterpolated = false;
    public bool backRightEdgeInterpolated = false;

    public bool generateColliders = false;

    public void SetMesh(List<Vector3> vertices, int[] triangles)
    {
        if (!setUp)
        {
            meshFilter = this.AddComponent<MeshFilter>();

            if (generateColliders)
            {
                meshCollider = this.AddComponent<MeshCollider>();
                meshRenderer = this.AddComponent<MeshRenderer>();
            }

            setUp = true;
        }

        Mesh mesh = new Mesh();
        mesh.name = "Chunk Mesh";

        mesh.Clear();

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);

        this.gameObject.layer = LayerMask.NameToLayer("Ground");

        meshFilter.mesh = mesh;

        mesh.RecalculateNormals();

        if (generateColliders)
        {
            meshCollider.sharedMesh = mesh;
        }

        meshIsSet = true;
    }

    public void Configure(Material mat)
    {
        GetComponent<MeshRenderer>().material = mat;
    }

    public void PrintVectors()
    {
        foreach (Vector3 vec in meshFilter.sharedMesh.vertices)
        {
            Debug.Log(vec);
        }
    }

    public void PrintTriangles()
    {
        foreach (int tri in meshFilter.mesh.triangles)
        {
            Debug.Log(tri);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = chunkBoundsColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(chunkSizeHorizontal, chunkSizeVertical, chunkSizeHorizontal));
    }
}