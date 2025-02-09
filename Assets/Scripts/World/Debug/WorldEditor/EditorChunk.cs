using System;
using System.Collections.Generic;
using UnityEngine;

public class EditorChunk : Chunk
{
    [Header("Chunk Parameters")]
    private Dictionary<Vector3Int, float> densities = new Dictionary<Vector3Int, float>();
    private Dictionary<Vector3Int, float> blendedDensities = new Dictionary<Vector3Int, float>();
    public Dictionary<Vector3Int, WorldEditorPoint> worldEditorPoints = new Dictionary<Vector3Int, WorldEditorPoint>();
    public bool WorldEditorPointsGenerated => worldEditorPointsGenerated;
    protected bool worldEditorPointsGenerated = false;

    public override void GenerateDensity() // Dont forget to release the density/points buffer for memory leaks
    {
        base.GenerateDensity();

        int numPointsPerAxis = World.Settings.numPointsPerAxis;
        int totalPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;

        Vector4[] _densities = new Vector4[totalPoints];
        densityBuffer.GetData(_densities);

        for (int x = 0; x < numPointsPerAxis; x++)
        {
            for (int y = 0; y < numPointsPerAxis; y++)
            {
                for (int z = 0; z < numPointsPerAxis; z++)
                {
                    Vector3Int point = new Vector3Int(x, y, z);

                    this.densities.Add(point, _densities[IndexFromCoord(point)].w);
                }
            }
        }
    }

    public override void FinalizeBlending()
    {
        finalizedBlendedBuffer = ongoingBlendedBuffer;

        int numPointsPerAxis = World.Settings.numPointsPerAxis;
        int totalPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;

        Vector4[] _blendedDensities = new Vector4[totalPoints];
        finalizedBlendedBuffer.GetData(_blendedDensities);

        for (int x = 0; x < numPointsPerAxis; x++)
        {
            for (int y = 0; y < numPointsPerAxis; y++)
            {
                for (int z = 0; z < numPointsPerAxis; z++)
                {
                    Vector3Int point = new Vector3Int(x, y, z);
                    float density = _blendedDensities[IndexFromCoord(point)].w;

                    if (densities[point] != density)
                    {
                        blendedDensities[point] = density;

                        WorldEditorPoint editorPoint = worldEditorPoints[point];
                        editorPoint.SetDensity(density);
                        editorPoint.SetColor(Color.red);
                    }
                }
            }
        }

        isBlended = true;
    }

    public void OverridePointDensity(Vector3 point, float density)
    {
        // TBA
    }

    public void RenderDensity()
    {
        for (int x = 0; x < World.Settings.numPointsPerAxis; x++)
        {
            for (int y = 0; y < World.Settings.numPointsPerAxis; y++)
            {
                for (int z = 0; z < World.Settings.numPointsPerAxis; z++)
                {
                    Vector3Int point = new Vector3Int(x, y, z);

                    
                    float offset = (float)World.Settings.chunkSize / 2 * -1;
                    Vector3 offsetVector = new Vector3(offset, offset, offset);

                    Vector3 positionOfPoint = new Vector3(x, y, z) * World.Settings.pointSpacing + offsetVector + transform.position;

                    GameObject newPointGO = Instantiate(WorldEditor.Instance.densityPointPrefab, positionOfPoint, Quaternion.identity, transform);
                    WorldEditorPoint newPoint = newPointGO.GetComponent<WorldEditorPoint>();
                    newPointGO.transform.position = positionOfPoint;
                    newPointGO.name = "Density Point " + point;

                    newPoint.SetDensity(densities[point]);
                    worldEditorPoints.Add(point, newPoint);
                }
            }
        }
    }

    private int IndexFromCoord(Vector3 coord)
    {
        int numPointsPerAxis = World.Settings.numPointsPerAxis;
        return (int)(coord.x + coord.y * numPointsPerAxis + coord.z * numPointsPerAxis * numPointsPerAxis);
    }
}