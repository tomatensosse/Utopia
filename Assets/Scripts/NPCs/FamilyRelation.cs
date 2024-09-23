using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FamilyRelation
{
    public string relationFamilyID;
    [Tooltip("Set between 0.0 - 1.0f; Lower means better relationship.")]
    [Range(0.0f, 1.0f)]
    public float relationValue;
}
