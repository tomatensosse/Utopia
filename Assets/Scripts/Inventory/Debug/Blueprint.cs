using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Blueprint", menuName = "Inventory/Blueprint")]
public class Blueprint : ScriptableObject
{
    [Header("Crafted Item")]
    public Item itemToCraft;
    public int craftYield;

    [Header("Materials")]
    public List<BlueprintMaterial> materials;

    [System.Serializable]
    public class BlueprintMaterial 
    { 
        public Item materialItem;
        public int materialItemAmount;
    }
}
