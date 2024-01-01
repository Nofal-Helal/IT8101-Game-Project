using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class ZombieScript : BaseUniversal
{
    public float playerProximityDistance = 5f;

    public float attackAnimationLength = 1.5f;
    public float attackDelay = 2f;
    private float originalAttackRange;

    private new void Start()
    {
        isPlayerCloseLogSent = false;
        animator = GetComponent<Animator>();
        originalAttackRange = attackRange;
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the object or its children.");
        }
    }

    protected override void Update()
    {
        base.Update();
        if (isAlive)
        {
            GameObject player = FindPlayer();
            if (player != null)
            {
                HandlePlayerProximity(player);
            }
        }
    }

    protected override void AttackPlayer(test_player_movement_script playerScript)
    {
        if (timeSinceLastAttack >= attackCooldown)
        {
            base.AttackPlayer(playerScript);
            timeSinceLastAttack = 0f;
            isAttacking = true;
            StartCoroutine(WaitForAttack(playerScript));
        }
    }

    IEnumerator WaitForAttack(test_player_movement_script playerScript)
    {
        yield return new WaitForSeconds(attackDelay);

        if (isAttacking && IsPlayerInRange(playerScript.transform.position))
        {
            TriggerAttackAnimation("BiteTrigger");
            AttackPlayer(playerScript);

            yield return new WaitForSeconds(attackAnimationLength);

            FinishAttack();
        }

        ResetAttack();
    }

    private void ResetAttack()
    {
        isAttacking = false;
        isPlayerCloseLogSent = false;
        UpdateAnimatorParameters();
        TriggerRunAnimation("RunTrigger");
        attackRange = originalAttackRange;
    }

    public override void FinishAttack()
    {
        base.FinishAttack();
        Invoke("ResetAttack", 0.5f);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        if (health <= 0 && isAlive)
        {
            TriggerDeathAnimation("DeathTrigger");
            Debug.Log("OOF");
            isAlive = false;
            base.Die();
        }
    }

    private GameObject FindPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    public override void HandlePlayerProximity(GameObject player)
    {
        if (player.CompareTag("Player"))
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= playerProximityDistance)
            {
                if (!isPlayerCloseLogSent)
                {
                    if (distanceToPlayer <= attackRange)
                    {
                        Debug.Log("I'm gonna bite ya!");
                        isPlayerCloseLogSent = true;
                        UpdateAnimatorParameters();
                        TriggerAttackAnimation("BiteTrigger");
                        AttackPlayer(player.GetComponent<test_player_movement_script>());
                    }
                    else
                    {
                        Debug.Log("Player is close to the zombie! Attacking...");
                        isPlayerCloseLogSent = true;
                        UpdateAnimatorParameters();
                        TriggerRunAnimation("RunTrigger");
                    }
                }

                MoveTowardsPlayer(player.transform.position);
            }
            else
            {
                if (isPlayerCloseLogSent)
                {
                    Debug.Log("Player is not close to the zombie. Going into idle.");
                    isPlayerCloseLogSent = false;
                    UpdateAnimatorParameters();
                    TriggerIdleAnimation("IdleTrigger");
                    isAttacking = false;
                }
            }
        }
        else
        {
            if (isPlayerCloseLogSent)
            {
                Debug.Log("Enemy: Player is not close. Going into idle.");
                isPlayerCloseLogSent = false;
                UpdateAnimatorParameters();
                TriggerIdleAnimation("IdleTrigger");
            }
        }
    }

    public override void UpdateAnimatorParameters()
    {
        if (animator != null)
        {
            animator.SetBool("isPlayerCloseLogSent", isPlayerCloseLogSent);
            animator.SetBool("BiteTrigger", isAttacking);
        }
    }

    public override void TriggerAttackAnimation(string attackTrigger)
    {
        if (isAlive && animator != null)
        {
            animator.SetTrigger(attackTrigger);
        }
    }
}
