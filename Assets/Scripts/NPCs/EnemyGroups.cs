using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyGroup", menuName = "NPCs/EnemyGroup")]
public class NPCGroup : ScriptableObject
{
    [Header("Group Settings")]
    public int maxMembers = 8; // Maksimum grup üyesi 
    public int healerCount = 2; // Healer 
    public int tankCount = 2;   // Tank 
    public int warriorCount = 3; // Warrior 
    public GameObject EnemyPrefab; // galiba gang'in hangi tür enemyden oluşacağını belirliyo ama valla bilmiyom
    private List<NPC> groupMembers = new List<NPC>(); 
    private NPC leader; // inş private olması doğrudur
}