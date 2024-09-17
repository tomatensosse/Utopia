using UnityEngine;

public class TerrainHeightGen : MonoBehaviour
{
    [HideInInspector]
    public MeshData meshData = new MeshData();
    [SerializeField] [Range(0f, 1f)] protected float strength = 1f;

    // Override this method in derived scripts
    public virtual MeshData Execute(int chunkSizeHorizontal, Vector2 chunkPosition, float seed)
    {
        Debug.LogError("No implementation for TerrainHeightGen.Execute() on " + this.name);
        return null;
    }
}
