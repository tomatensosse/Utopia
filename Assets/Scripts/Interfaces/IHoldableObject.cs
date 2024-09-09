using UnityEngine;

public interface IHoldableObject
{
    void PickUp(Transform player);
    void Drop();
    void Throw(Vector3 throwForce);
    void Rotate(Vector3 rotationDelta);
}
