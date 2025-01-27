using UnityEngine;

public class CameraController : MonoBehaviour
{
   [Header("Camera Settings")]
   [SerializeField] private Transform target;
   [SerializeField] private float distance = 10f;
   [SerializeField] private float sensitivity = 2f;
   [SerializeField] private float zoomSpeed = 5f;
   [SerializeField] private float minDistance = 5f;
   [SerializeField] private float maxDistance = 30f;
   
   [Header("Smoothing")]
   [SerializeField] private float rotationSmoothTime = 0.12f;
   [SerializeField] private float zoomSmoothTime = 0.1f;
   
   private float currentX = 0f;
   private float currentY = 0f;
   private float targetX = 0f;
   private float targetY = 0f;
   private float targetDistance;
   
   private Vector2 rotationVelocity;
   private float zoomVelocity;

   void Start()
   {
       if (target == null && transform.parent != null)
           target = transform.parent;
           
       Vector3 angles = transform.eulerAngles;
       currentX = targetX = angles.y;
       currentY = targetY = angles.x;
       targetDistance = distance;
   }

   void LateUpdate()
   {
       if (target == null) return;

       if (Input.GetMouseButton(1))
       {
           targetX += Input.GetAxis("Mouse X") * sensitivity;
           targetY -= Input.GetAxis("Mouse Y") * sensitivity;
           targetY = Mathf.Clamp(targetY, -85f, 85f);
       }

       targetDistance = Mathf.Clamp(targetDistance - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, 
           minDistance, maxDistance);

       // Smooth rotations
       currentX = Mathf.SmoothDampAngle(currentX, targetX, ref rotationVelocity.x, rotationSmoothTime);
       currentY = Mathf.SmoothDampAngle(currentY, targetY, ref rotationVelocity.y, rotationSmoothTime);
       
       // Smooth zoom
       distance = Mathf.SmoothDamp(distance, targetDistance, ref zoomVelocity, zoomSmoothTime);

       Vector3 dir = new Vector3(0, 0, -distance);
       Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
       transform.position = target.position + rotation * dir;
       transform.LookAt(target.position);
   }
}