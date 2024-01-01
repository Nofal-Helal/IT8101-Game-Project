using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class SkeletonScript : BaseUniversal
{
    public float playerProximityDistance;
    public float attackDelay = 2f;
    // private bool isAttacking = false;
    public float attackAnimationLength = 1.5f;
    private bool isCollidingWithPlayer = false;
    private new void Start()
    {
        isPlayerCloseLogSent = false;
        animator = GetComponent<Animator>();
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
            animator.SetTrigger("AttackTrigger");

            // Apply damage after attackDelay
            StartCoroutine(WaitForAttack(playerScript));
        }
    }

    IEnumerator WaitForAttack(test_player_movement_script playerScript)
    {
        // Wait for the attackDelay
        yield return new WaitForSeconds(attackDelay);

        // Check if still attacking and player is within attack range
        if (isAttacking && IsPlayerInRange(playerScript.transform.position) && isCollidingWithPlayer)
        {
            // Player is in range, initiate attack
            TriggerAttackAnimation("AttackTrigger");
            Debug.Log("Attack animation triggered");

            // Wait for the duration of the attack animation
            yield return new WaitForSeconds(attackAnimationLength); // Adjust with the actual length of your attack animation

            // Finish the attack
            FinishAttack();
        }

        // Reset the attack state
        ResetAttack();
    }

    protected void ResetAttack()
    {
        Debug.Log("ResetAttack");
        isAttacking = false;
        isPlayerCloseLogSent = false;
        isCollidingWithPlayer = false;
        UpdateAnimatorParameters();
        TriggerRunAnimation("WalkTrigger");
    }


    public override void FinishAttack()
    {
        Debug.Log("Finished attacking, time to chillax");
        isPlayerCloseLogSent = false;
        isAttacking = false;
        UpdateAnimatorParameters();
        animator.ResetTrigger("AttackTrigger");
        TriggerRunAnimation("WalkTrigger");
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
        }
    }

    private GameObject FindPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    public override void HandlePlayerProximity(GameObject player)
    {
        if (player.gameObject.tag == "Player")
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= playerProximityDistance)
            {
                if (!isPlayerCloseLogSent)
                {
                    Debug.Log("Player is close to the zombie! Chasing...");
                    isPlayerCloseLogSent = true;
                    UpdateAnimatorParameters();
                }

                // Move towards the player
                MoveTowardsPlayer(player.transform.position);

                if (IsPlayerInRange(player.transform.position) && !isAttacking)
                {
                    // Player is close enough, initiate attack
                    isAttacking = true; // Set isAttacking to true
                    StartCoroutine(WaitForAttack(player.GetComponent<test_player_movement_script>()));
                    TriggerAttackAnimation("AttackTrigger");
                }
                else
                {
                    // Player is not close enough for attack, keep chasing
                    TriggerRunAnimation("WalkTrigger");
                }
            }
            else
            {
                if (isPlayerCloseLogSent)
                {
                    Debug.Log("Player is not close to the zombie. Going into idle.");
                    isPlayerCloseLogSent = false;
                    UpdateAnimatorParameters();
                    TriggerIdleAnimation("IdleTrigger");
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            test_player_movement_script playerScript = collision.gameObject.GetComponent<test_player_movement_script>();
            if (playerScript != null && playerScript.IsAlive())
            {
                // Player collided with the skeleton, initiate attack
                isAttacking = true;
                isCollidingWithPlayer = true;
                AttackPlayer(playerScript);
            }
        }
    }

    // private void TriggerAttackAnimation()
    // {
    //     if (isAlive && animator != null && CanAttack())
    //     {
    //         animator.SetTrigger("AttackTrigger");
    //     }
    // }

    // Animation event function called when the attack animation deals damage
    public override void DealDamage()
    {
        GameObject player = FindPlayer();
        if (player != null)
        {
            test_player_movement_script playerScript = player.GetComponent<test_player_movement_script>();
            if (playerScript != null && playerScript.IsAlive() && IsPlayerInRange(player.transform.position))
            {
                playerScript.TakeDamage(damage);
            }
        }
    }

    public override void UpdateAnimatorParameters()
    {
        if (animator != null)
        {
            animator.SetBool("isPlayerCloseLogSent", isPlayerCloseLogSent);
            animator.SetBool("AttackTrigger", isAttacking); // Update the AttackTrigger with isAttacking
        }
    }

    private void TriggerRunAnimation()
    {
        animator.ResetTrigger("AttackTrigger"); // Reset the AttackTrigger
        animator.SetTrigger("WalkTrigger");
    }

    public override void TriggerIdleAnimation(string triggerName)
    {
        animator.SetTrigger("IdleTrigger");
    }
}
