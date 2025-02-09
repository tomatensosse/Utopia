using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceActiveCamera : MonoBehaviour
{
    private Transform activeCamera;

    void Update()
    {
        if (activeCamera == null)
        {
            activeCamera = Camera.main.transform;
        }

        /*
        if (DensityInspector.Instance != null && DensityInspector.Instance.inspectorCamera != null)
        {
            activeCamera = DensityInspector.Instance.inspectorCamera.transform;
        }
        */
        
        transform.LookAt(activeCamera);
    }
}
