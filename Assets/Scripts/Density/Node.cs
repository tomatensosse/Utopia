using UnityEngine;

[System.Serializable]
public abstract class Node
{
    public abstract ComputeBuffer GenerateDensity(Vector3 worldPositionOfChunk);
}