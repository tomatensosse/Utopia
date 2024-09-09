using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Camera Move")]
    public Transform cameraPosition;
    public bool active = false;

    [Header("Other")]
    public Transform holdableObjectContainer;

    private void Update()
    {
        if (!active) { return;}
        
        transform.position = cameraPosition.position;
    }
}