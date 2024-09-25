using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyGroup", menuName = "NPCs/EnemyGroup")]

// grup taktikleri belirle defans lideiKoruma, saldırma, kaçma, yanıltma
public enum Tactic {defence, protect, attack, evasive, deceptive}

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

    public override void Start()
    {
        base.Start();
        assignLeader();
        setMemberRoles();,
        spawnGroup();
    }

    public override void AssignLeader()
    {
        if (groupMembers.Count == 0)
            return;

        leader = groupMembers[Random.Range(0, groupMembers.Count)];
        leader.isLeader = true; 
        Debug.Log("Leader assigned: " + leader.gameObject.id);
    }
    public override void SetMemberRoles()
    {

    }
    public void SpawnGroup()
    {

    }
    public override void ProtectLeader()
    {
        // lideri koruma
    }
    public override void Defence()
    {
        // savunma mekanik
    }
}