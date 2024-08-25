using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    // Also going to work as an interactable.cs

    public Collider itemCollider;
    public Item item;
    public DynamicItem dynamicItem;
    public bool itemBeingHeld = false;
    [Header("Finger Tip Targets // ASSIGN CAREFULLY")]
    public Transform[] itemHoldPoints;

    public virtual void Start()
    {
        itemCollider = GetComponent<Collider>();
    }

    public virtual void PickupItem()
    {
        itemBeingHeld = true;
        itemCollider.enabled = false;
    }

    public virtual void HoldItem()
    {
        itemBeingHeld = true;
        itemCollider.enabled = true;
    }

}
