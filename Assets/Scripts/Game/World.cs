using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class World : NetworkBehaviour
{
    public static World Instance { get; private set; }
    public WorldData worldData;
    [HideInInspector] public bool initialized;

    private void Awake()
    {
        Debug.Log("World Awake");

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There are multiple World instances in the scene.");
            Destroy(gameObject);
        }

        initialized = true;
    }

    private void Start()
    {
        
    }
}
