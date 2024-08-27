using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Blueprint", menuName = "Inventory/Blueprint")]
public class Blueprint : ScriptableObject
{
    public string itemToCraft;
    public List<BlueprintMaterial> materials;
    
    [System.Serializable]
    public class BlueprintMaterial 
    { 
        public string materialID;
        public int materialAmount;
    }
}
