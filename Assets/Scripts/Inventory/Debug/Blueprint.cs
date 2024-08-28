using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Blueprint", menuName = "Inventory/Blueprint")]
public class Blueprint : ScriptableObject
{
    public Item itemToCraft;
    public List<BlueprintMaterial> materials;
    
    [System.Serializable]
    public class BlueprintMaterial 
    { 
        public Item materialItem;
        public int materialItemAmount;
    }
}
