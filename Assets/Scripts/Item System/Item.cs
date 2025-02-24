using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Utopia/Item")]
public class Item : ScriptableObject
{
    public string uid;
    public string itemName;
    public Sprite icon;

    public bool isStackable;
    [ShowIf("isStackable")] public int maxStack;

    public bool isEquippable;
    [ShowIf("isEquippable")] public InventorySlotType equipSlot = InventorySlotType.Storage;

    public bool isHoldable;
    [ShowIf("isHoldable")] public bool isTwoHanded;

    [SerializeReference] public List<PlayerAbilityType> abilities = new List<PlayerAbilityType>();
    // List<ItemModification> modifications; bla bla bla
}