using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class ChunksHolder : MonoBehaviour
{
    public static ChunksHolder Instance { get; private set; }

    public NavMeshSurface navMeshSurface;
    public LayerMask layerMask; // Which layers to include in the NavMesh
    private AsyncOperation navMeshAsyncOperation;

    private void Start()
    {
        Instance = this;

        navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
        layerMask = LayerMask.GetMask("Ground");
    }

    // Call this method to build the NavMesh across all chunks
    public void BuildNavMesh()
    {
        // Collect NavMesh sources from all the chunks
        List<NavMeshBuildSource> sources = CollectNavMeshSources();

        // Calculate the bounds for the combined area of the chunks
        Bounds bounds = CalculateWorldBounds();

        // Build the NavMesh asynchronously
        if (navMeshSurface.navMeshData == null)
        {
            navMeshSurface.navMeshData = new NavMeshData();
        }

        // Update the NavMesh
        navMeshAsyncOperation = NavMeshBuilder.UpdateNavMeshDataAsync(navMeshSurface.navMeshData, navMeshSurface.GetBuildSettings(), sources, bounds);

        // Register a callback for when the NavMesh has been updated
        navMeshAsyncOperation.completed += OnNavMeshUpdated;
    }

    private void OnNavMeshUpdated(AsyncOperation asyncOperation)
    {
        StartCoroutine(Workaround());

        Debug.Log("NavMesh has been updated!");

        // Notify all NavMeshAgents to reset or recalculate their paths
        NavMeshAgent[] agents = FindObjectsOfType<NavMeshAgent>();

        foreach (NavMeshAgent agent in agents)
        {
            if (agent.isOnNavMesh)
            {
                // Set the destination again to force the agent to update its path
                agent.SetDestination(agent.destination);
            }
        }
    }

    private IEnumerator Workaround()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.001f, transform.position.z);
        yield return new WaitForSeconds(0.1f);
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.001f, transform.position.z);
    }

    // Collects NavMesh sources from all chunks (assuming chunks are children of this GameObject)
    private List<NavMeshBuildSource> CollectNavMeshSources()
    {
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        foreach (Transform child in transform)
        {
            Chunk chunk = child.GetComponent<Chunk>();
            if (chunk != null && chunk.meshIsSet)
            {
                MeshFilter meshFilter = chunk.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    // Create a NavMeshBuildSource for this chunk
                    NavMeshBuildSource source = new NavMeshBuildSource
                    {
                        shape = NavMeshBuildSourceShape.Mesh,
                        sourceObject = meshFilter.sharedMesh,
                        transform = meshFilter.transform.localToWorldMatrix,
                        area = 0 // Use the default area
                    };

                    sources.Add(source);
                }
            }

            // Check chunk children for spawnables
            foreach (Transform spawnable in child)
            {
                Spawnable spawnableComponent = spawnable.GetComponent<Spawnable>();
                if (spawnableComponent != null)
                {
                    // Create a NavMeshBuildSource for this spawnable
                    NavMeshBuildSource source = new NavMeshBuildSource
                    {
                        shape = NavMeshBuildSourceShape.Mesh,
                        sourceObject = spawnable.GetComponent<MeshFilter>().sharedMesh,
                        transform = spawnable.transform.localToWorldMatrix,
                        area = 0 // Use the default area
                    };

                    sources.Add(source);
                }
            }
        }

        return sources;
    }

    // Calculates the world bounds of the combined chunks
    private Bounds CalculateWorldBounds()
    {
        Bounds combinedBounds = new Bounds(Vector3.zero, Vector3.zero);
        bool initialized = false;

        foreach (Transform child in transform)
        {
            Chunk chunk = child.GetComponent<Chunk>();
            if (chunk != null)
            {
                Vector3 chunkCenter = child.position;
                Vector3 chunkSize = new Vector3(chunk.chunkSizeHorizontal, chunk.chunkSizeVertical, chunk.chunkSizeHorizontal);
                Bounds chunkBounds = new Bounds(chunkCenter, chunkSize);

                if (!initialized)
                {
                    combinedBounds = chunkBounds;
                    initialized = true;
                }
                else
                {
                    combinedBounds.Encapsulate(chunkBounds);
                }
            }
        }

        return combinedBounds;
    }

    // For debugging, draw the bounds of the combined chunks in the Scene view
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Bounds bounds = CalculateWorldBounds();
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}
