using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class NPC : Entity, INPCInteractionHandler
{
    [Header("NPC Interaction")]
    public Family family;
    public float toleranceVariation;
    public float tolerance;
    public float relation;

    public Family Family { get; set; }
    public float ToleranceVariation { get; set; }
    public float Tolerance { get; set; }
    public float Relation { get; set; }

    [Header("Behaviour")]
    public bool isAgentDriven;
    [ConditionalHide(nameof(isAgentDriven), true)]
    public NavMeshAgent nma;

    [Header("Behaviour Variables")]
    public float minWanderDistance = 25f;
    public float maxWanderDistance = 50f;
    public float idleMinTime = 1f;
    public float idleMaxTime = 5f;
    public float attackInitRange = 2f;
    public float attackAngle = 45f;
    public float attackRange = 4f;
    public float attackAfter = 1f;
    public float attackMoveAfterInit = 0.5f;
    public float attackCooldown = 1f;
    private float attackCooldownTimer = 0;
    public int attackDamage = 3;
    public float viewMaxDistance = 15f;
    public float loseTargetAtDistance = 20f;
    public float viewMaxAngle = 45f;
    private bool hasActiveDestination = false;
    private bool atTargetDestination = false;
    private Transform targeted;
    private bool hasTarget = false;

    [Header("Animation")]
    public Animator anim;

    public override void Awake()
    {
        base.Awake();

        // Assign accessor values to the interface properties

        Family = family;
        ToleranceVariation = toleranceVariation;
        Tolerance = tolerance;
        Relation = relation;

        if (nma == null)
        {
            nma = GetComponent<NavMeshAgent>();
        }
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    public override void Start()
    {
        base.Start();

        if (isAgentDriven)
        {
            nma = GetComponent<NavMeshAgent>();
        }
        else
        {
            rb = GetComponent<Rigidbody>();
        }

        Wander();
    }

    public virtual void Update()
    {
        CheckAtTarget();
        FindTarget();
    }

    public virtual void Die()
    {
        anim.enabled = false;

        if (isAgentDriven)
        {
            nma.enabled = false;
        }
        else
        {
            // Disable rb
        }

        NetworkServer.Destroy(gameObject);
    }

    public virtual void MoveTo(Vector3 target)
    {
        if (isAgentDriven)
        {
            nma.SetDestination(target);
        }
        else
        {
            // Move manually
        }
    }

    public virtual void Wander()
    {
        if (hasActiveDestination || targeted != null)
        {
            return;
        }

        nma.isStopped = false;

        Vector3 randomDirection = Random.insideUnitSphere * Random.Range(minWanderDistance, maxWanderDistance);
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, maxWanderDistance, 1);
        Vector3 finalPosition = hit.position;

        MoveTo(finalPosition);
        hasActiveDestination = true;

        anim.SetBool("idle", false);
    }

    public virtual void CheckAtTarget()
    {
        if (Vector3.Distance(transform.position, nma.destination) < 0.25f && !atTargetDestination)
        {
            hasActiveDestination = false;
            anim.SetBool("idle", true);
            atTargetDestination = true;
            StartCoroutine(Idle());
        }
    }

    public virtual IEnumerator Idle()
    {
        yield return new WaitForSeconds(Random.Range(idleMinTime, idleMaxTime));
        atTargetDestination = false;
        Wander();
    }

    public virtual void FindTarget()
    {
        if (hasTarget)
        {
            // Check if target is still in sight
            if (Vector3.Distance(transform.position, targeted.position) > loseTargetAtDistance)
            {
                hasTarget = false;
                hasActiveDestination = false;
                targeted = null;
            }
            else
            {
                INPCInteractionHandler targetHandler = targeted.GetComponent<INPCInteractionHandler>();
                if (targetHandler != null)
                {
                    if (Family.relationsDict.TryGetValue(targetHandler.Family.familyID, out float relationValue))
                    {
                        Behave(relation); // Make the behaviour function take 2 params of (float relation, type enemy) if enemy type specific attacks needed
                    }
                }
            }
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewMaxDistance);

        foreach (Collider hitCollider in hitColliders)
        {
            INPCInteractionHandler hitHandler = hitCollider.GetComponent<INPCInteractionHandler>();
            if (hitHandler != null)
            {
                if (Family.relationsDict.TryGetValue(hitHandler.Family.familyID, out float relationValue))
                {
                    targeted = hitCollider.transform;
                    hasTarget = true;

                    Behave(relation);
                }
            }
        }

        if (!hasTarget && !hasActiveDestination)
        {
            Wander();
        }
    }

    public virtual void Behave(float relation)
    {
        StopCoroutine(Idle());

        if (relation < tolerance)
        {
            Neutral();
        }
        else
        {
            Aggro();
        }
    }

    public virtual void Neutral()
    {
        // Excecuted when the relation is more than the tolerance

        anim.SetBool("idle", true);
        nma.isStopped = true;
        LookAtTarget();
    }

    public virtual void Aggro()
    {
        if (Time.time > attackCooldownTimer)
        {
            attackCooldownTimer = Time.time + attackCooldown + attackMoveAfterInit;
            StartCoroutine(Attack());
        }

        if (!(Time.time > attackCooldownTimer))
        {
            LookAtTarget();
        }
    }

    public virtual void LookAtTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(targeted.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    public virtual bool EnemyInAttackRange(bool initRange)
    {
        if (initRange)
        {
            if (Vector3.Distance(transform.position, targeted.position) < attackInitRange)
            {
                if (Vector3.Angle(transform.forward, targeted.position - transform.position) < attackAngle)
                {
                    return true;
                }
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, targeted.position) < attackRange)
            {
                if (Vector3.Angle(transform.forward, targeted.position - transform.position) < attackAngle)
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    public virtual IEnumerator Attack()
    {
        yield return new WaitForSeconds(attackMoveAfterInit);

        anim.SetTrigger("attack");

        nma.isStopped = true;

        yield return new WaitForSeconds(attackAfter);

        // Check if enemy in attack damage range
        if (!EnemyInAttackRange(false))
        {
            yield break;
        }

        // Change to entity
        IHealth healthHandler = targeted.GetComponent<IHealth>();
        INPCInteractionHandler interactionHandler = targeted.GetComponent<INPCInteractionHandler>();
        if (healthHandler != null)
        {
            healthHandler.CmdTakeDamage(attackDamage, null); // Maybe change null to connectionToClient
            interactionHandler.Family.AddRelation(Family.familyID, 0.1f);
        }

        yield return new WaitForSeconds(attackCooldown - attackAfter);

        nma.isStopped = false;
    }

#if UNITY_EDITOR
    [ContextMenu("Debug Damage")]
    public void DebugDamage()
    {
        CmdTakeDamage(5, null);
    }
#endif
}
