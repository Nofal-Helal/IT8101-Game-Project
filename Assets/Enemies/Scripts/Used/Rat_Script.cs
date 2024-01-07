using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class Rat_Script : BaseUniversal, IDamageTaker
{
    // Proximity distance to detect the player
    public float playerProximityDistance;

    // Delay before attacking after detecting the player
    public float attackDelay = 2f;

    // Duration of the attack animation
    public float attackAnimationLength = 1.5f;

    // Flag to check if the rat is colliding with the player
    private bool isCollidingWithPlayer = false;

    // Initialization
    private new void Start()
    {
        isPlayerCloseLogSent = false;
        animator = GetComponent<Animator>();

        // Check if the Animator component is present
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the object or its children.");
        }
    }

    // Update is called once per frame
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

    // Attack the player
    protected override void AttackPlayer(Player playerScript)
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

    // Coroutine to wait for the attackDelay before initiating the attack
    IEnumerator WaitForAttack(Player playerScript)
    {
        // Wait for the attackDelay
        yield return new WaitForSeconds(attackDelay);

        // Check if still attacking and player is within attack range and colliding
        if (isAttacking && IsPlayerInRange(playerScript.transform.position) && isCollidingWithPlayer)
        {
            // Player is in range, initiate attack
            TriggerAttackAnimation("AttackTrigger");
            DealDamage(damage); // Call DealDamage to deal damage to the player
            Debug.Log("Attack animation triggered");

            // Wait for the duration of the attack animation
            yield return new WaitForSeconds(attackAnimationLength); // Adjust with the actual length of your attack animation

            // Finish the attack
            FinishAttack();
        }

        // Reset the attack state
        ResetAttack();
    }

    // Reset the attack state
    protected void ResetAttack()
    {
        Debug.Log("ResetAttack");
        isAttacking = false;
        isPlayerCloseLogSent = false;
        isCollidingWithPlayer = false;
        UpdateAnimatorParameters();
        TriggerRunAnimation("WalkTrigger");
    }

    // Finish the attack
    public override void FinishAttack()
    {
        Debug.Log("Finished attacking, time to chillax");
        isPlayerCloseLogSent = false;
        isAttacking = false;
        UpdateAnimatorParameters();
        animator.ResetTrigger("AttackTrigger");
        TriggerRunAnimation("WalkTrigger");
    }

    // Take damage
    // implemented in base
    // void IDamageTaker.TakeDamage(float damage) { }

    // Handle death
    protected override void Die()
    {
        if (health <= 0 && isAlive)
        {
            TriggerDeathAnimation("DeathTrigger");
            Debug.Log("OOF");
        }
        //Base Death Method
        base.Die();
    }

    // Find the player in the scene
    private GameObject FindPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    // Handle player proximity and initiate actions accordingly
    public override void HandlePlayerProximity(GameObject player)
    {
        if (player.gameObject.tag == "Player")
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= playerProximityDistance)
            {
                if (!isPlayerCloseLogSent)
                {
                    Debug.Log("Player is close to the rat! Chasing...");
                    isPlayerCloseLogSent = true;
                    UpdateAnimatorParameters();
                }

                // Move towards the player
                MoveTowardsPlayer(player.transform.position);

                // Check if the player is within the attack range
                if (IsPlayerInRange(player.transform.position) && !isAttacking)
                {
                    // Player is close enough, initiate attack
                    isAttacking = true; // Set isAttacking to true
                    StartCoroutine(WaitForAttack(player.GetComponent<Player>()));
                    TriggerAttackAnimation("AttackTrigger");
                }
                else if (isAttacking && !IsPlayerInRange(player.transform.position))
                {
                    // Player is not in range, stop attacking
                    isAttacking = false;
                    ResetAttack();
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
                    Debug.Log("Player is not close to the rat. Going into idle.");
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

    // Handle collision with player
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player playerScript = collision.gameObject.GetComponent<Player>();
            if (playerScript != null && playerScript.isAlive())
            {
                // Player collided with the rat, initiate attack
                isAttacking = true;
                isCollidingWithPlayer = true;
                AttackPlayer(playerScript);
            }
        }
    }

    // Update animator parameters
    public override void UpdateAnimatorParameters()
    {
        if (animator != null)
        {
            animator.SetBool("isPlayerCloseLogSent", isPlayerCloseLogSent);
            animator.SetBool("AttackTrigger", isAttacking); // Update the AttackTrigger with isAttacking
        }
    }

    // Trigger the run animation
    private void TriggerRunAnimation()
    {
        animator.ResetTrigger("AttackTrigger"); // Reset the AttackTrigger
        animator.SetTrigger("WalkTrigger");
    }

    // Trigger the idle animation
    public override void TriggerIdleAnimation(string triggerName)
    {
        animator.SetTrigger("IdleTrigger");
    }
}
