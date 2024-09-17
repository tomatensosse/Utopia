using System.Collections;
using UnityEngine;

public interface IHoldableObject
{
    bool IsHeld { get; }
    bool OnCooldown { get; }
    void Hold(Transform player);
    void UnHold();
    IEnumerator HoldCooldown();
    void Throw(Vector3 throwForce);
    void Rotate(Vector3 rotationDelta);
}
