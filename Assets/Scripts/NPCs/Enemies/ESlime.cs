using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESlime : NPC
{
    [Header("Enemy Specific Variables")]
    public float aggressionLevel = 5f;
    public float rangedAttackDistance = 10f;
    public int rangedAttackDamage = 5;

    public override void Start()
    {
        base.Start();
        // baslangic ayarlari
        Debug.Log("Enemy has spawned with aggression level: " + aggressionLevel);
    }

    public override void Update()
    {
        base.Update();

        if (hasTarget)
        {
            // hefed ve alan var sa ozel attack
            if (Vector3.Distance(transform.position, targeted.position) < rangedAttackDistance)
            {
                PerformRangedAttack();
            }
        }
    }

    
    public override void Aggro()
    {
        if (Vector3.Distance(transform.position, targeted.position) < attackRange)
        {
            base.Aggro(); // Yakin saldiri
        }
        else if (Vector3.Distance(transform.position, targeted.position) < rangedAttackDistance)
        {
            PerformRangedAttack(); // Uzak saldiri
        }
    }

    // elemana ozel uzak mesafe saldiri
    public void PerformRangedAttack()
    {
        Debug.Log("Enemy performing a ranged attack!");
        anim.SetTrigger("rangedAttack");

        // Ranged attack mechanics ok yada silah mermi cart curt
        IHealth healthHandler = targeted.GetComponent<IHealth>();
        if (healthHandler != null)
        {
            healthHandler.CmdTakeDamage(rangedAttackDamage, null); // Hedefe hasar ver
        }
    }
}
