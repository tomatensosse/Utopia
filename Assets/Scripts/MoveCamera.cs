using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;
    public bool active = false;

    private void Update()
    {
        if (!active) { return;}
        
        transform.position = cameraPosition.position;
    }
}