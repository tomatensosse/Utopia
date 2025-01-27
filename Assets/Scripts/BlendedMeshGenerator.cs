using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendedMeshGenerator : MonoBehaviour
{
    public static BlendedMeshGenerator Instance { get; private set; }
    public static bool Ready => Instance._isReady;

    private bool _isReady = false;

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

    public Mesh GenerateBlendedMesh()
    {
        return null;
    }

    private void CreateBuffers()
    {
        // Create buffers here

        Debug.Log("Created buffers | isReady!");
        _isReady = true;
    }
}
