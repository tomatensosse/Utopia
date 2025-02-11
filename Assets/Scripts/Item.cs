using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Utopia/Item")]
public class Item : ScriptableObject
{
    public string uid;
    public string itemName;
    [SerializeReference] public List<PlayerAbilityType> abilities = new List<PlayerAbilityType>();
}