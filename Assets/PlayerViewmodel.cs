using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerViewmodel : MonoBehaviour
{
    public static PlayerViewmodel Instance;

    [Header("Finger Target Transforms")]
    public SkinnedMeshRenderer viewmodelRenderer;
    public Transform[] fingerTipTargets; // Targets for each finger tip
    public Transform objectToHold; // The object the hand is currently holding

    private void Awake()
    {
        Instance = this;
    }

    public void AdjustFingerTargets(Transform[] newFingerTipTargets)
    {
        foreach (Transform fingerTarget in fingerTipTargets)
        {
            fingerTarget.position = newFingerTipTargets[0].position;
        }
    }

    public void DisableViewmodel()
    {
        viewmodelRenderer.enabled = false;
    }

    public void EnableViewmodel()
    {
        viewmodelRenderer.enabled = true;
    }

    /*
    void AutoAdjustFingerTargets()
    {
        Collider objectCollider = objectToHold.GetComponent<Collider>();
        if (objectCollider == null) return;

        // Iterate through each finger target
        foreach (Transform fingerTarget in fingerTipTargets)
        {
            // Adjust finger target to closest point on the object
            Vector3 closestPoint = objectCollider.ClosestPoint(fingerTarget.position);
            fingerTarget.position = closestPoint;

            Debug.Log("Finger Target Position: " + fingerTarget.position);
            Debug.Log("Closest Point: " + closestPoint);
        }
    }
    */
}
