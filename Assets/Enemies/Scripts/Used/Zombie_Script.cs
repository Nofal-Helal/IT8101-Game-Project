using System.Collections;
using UnityEngine;

public class ZombieScript : BaseUniversal, IDamageTaker
{
    // Proximity distance to detect the player
    // in base
    // public float playerProximityDistance = 5f;

    // Duration of the attack animation
    public float attackAnimationLength = 1.5f;

    // Delay before initiating an attack
    public float attackDelay = 2f;
    private float cooldown;

    // Original attack range to reset after an attack
    // in base
    // private float originalAttackRange;

    // Initialization at the start of the script
    private new void Start()
    {
        // Call the base class Start method
        base.Start();
        // Initialize variables
        isPlayerCloseLogSent = false;
        animator = GetComponent<Animator>();
        originalAttackRange = attackRange;
        cooldown = base.attackCooldown;

        // Display an error if Animator component is not found
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the object or its children.");
        }
    }

    // Update method overridden from the base class
    protected override void Update()
    {
        // Call the base class Update method
        base.Update();
        // Debug.Log(Vector3.Distance(base.playerCamera.transform.position, gameObject.transform.position));
        // Check if the zombie is alive
        if (isAlive)
        {
            // Find the player in the scene
            GameObject player = FindPlayer();

            // If the player is found, handle the proximity
            if (player != null)
            {
                HandlePlayerProximity();
            }
        }
    }
    // Method to attack the player
    protected override void AttackPlayer(Player playerScript)
    {
        // Check if enough time has passed since the last attack
        if (timeSinceLastAttack >= attackCooldown)
        {
            // Call the base class AttackPlayer method
            base.AttackPlayer(playerScript);

            // Reset the attack timer
            timeSinceLastAttack = 0f;

            // Set the zombie in attacking state
            isAttacking = true;

            // Start the attack coroutine
            StartCoroutine(Attack());
            // Call the DealDamage method with the damage parameter
            DealDamage(damage);
        }
    }
    //Coroutines are commonly employed for tasks that require waiting for a specific duration or certain events to happen.
    // Coroutine to wait for the attack sequence to finish
    IEnumerator Attack()
    {
        // Wait for the specified delay before initiating the attack
        yield return new WaitForSeconds(attackDelay);
        // Check if the zombie is still attacking and the player is in range
        if (isAttacking && IsPlayerInRange(playerCamera.transform.position))
        {
            // Trigger the attack animation
            TriggerAttackAnimation("BiteTrigger");
            // Continue the attack sequence
            if (player != null && player.isAlive)
            {
                // Call TakeDamage method of the player
                ((IDamageTaker)player).TakeDamage(damage);
            }
            // Wait for the attack animation to finish
            yield return new WaitForSeconds(attackAnimationLength);
            // Finish the attack
            FinishAttack();
        }

        // Reset the attack state
        ResetAttack();
    }

    // Method to reset the attack state
    private void ResetAttack()
    {
        // Set the attacking state and player close log to false
        isAttacking = false;
        isPlayerCloseLogSent = false;

        // Update animator parameters and trigger the run animation
        UpdateAnimatorParameters();
        TriggerRunAnimation("RunTrigger");

        // Reset the attack range to its original value
        attackRange = originalAttackRange;
    }

    // Method called after the attack is finished
    public override void FinishAttack()
    {
        // Call the base class FinishAttack method
        base.FinishAttack();

        // Invoke the ResetAttack method after a short delay
        Invoke("ResetAttack", 0.5f);
    }

    // Take damage
    // implemented in base
    // void IDamageTaker.TakeDamage(float damage) { }

    // Method called when the zombie dies
    protected override void Die()
    {
        // Check if health is zero and the zombie is still alive
        if (health <= 0 && isAlive)
        {
            // Trigger the death animation
            TriggerDeathAnimation("DeathTrigger");

            // Set the zombie as dead
            isAlive = false;
            if (!isAlive)
            {
                //Die function from base class
                base.Die();
            }
        }
    }

    // Method to find the player in the scene
    private GameObject FindPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    // Method to handle the proximity of the player
    public override void HandlePlayerProximity()
    {
        // Check if the player tag is "Player"
        if (player.CompareTag("Player"))
        {
            // Calculate the distance to the player
            float distanceToPlayer = Vector3.Distance(transform.position, playerCamera.transform.position);
            // Debug.Log(distanceToPlayer);

            // Check if the player is within proximity
            if (distanceToPlayer <= playerProximityDistance)
            {
                // Check if the player close log hasn't been sent
                // If the player is within attack range, log an attack message
                if (distanceToPlayer <= attackRange)
                {
                    isPlayerCloseLogSent = true;
                    UpdateAnimatorParameters();
                    TriggerAttackAnimation("BiteTrigger");
                    cooldown -= Time.deltaTime;

                    if (player != null && player.isAlive && cooldown <= 0)
                    {
                        ((IDamageTaker)player).TakeDamage(damage);
                        cooldown = base.attackCooldown;
                    }
                    // AttackPlayer(player.GetComponent<Player>());
                }
                // If the player is close but not within attack range, log a chase message
                else
                {
                    isPlayerCloseLogSent = true;
                    UpdateAnimatorParameters();
                    TriggerRunAnimation("RunTrigger");
                    base.MoveTowardsPlayer(playerCamera.transform.position);
                }
                // Move towards the player's position
                base.MoveTowardsPlayer(playerCamera.transform.position);
            }
            else
            {
                // If the player was close, log that the player is not close anymore
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
            // If the player was close, log that the enemy (non-player) is not close anymore
            if (isPlayerCloseLogSent)
            {
                Debug.Log("Enemy: Player is not close. Going into idle.");
                isPlayerCloseLogSent = false;
                UpdateAnimatorParameters();
                TriggerIdleAnimation("IdleTrigger");
            }
        }
    }

    // Method to update animator parameters
    public override void UpdateAnimatorParameters()
    {
        // Check if the animator component is not null
        if (animator != null)
        {
            // Set boolean parameters in the animator for controlling animations
            animator.SetBool("isPlayerCloseLogSent", isPlayerCloseLogSent);
            animator.SetBool("BiteTrigger", isAttacking);
        }
    }

    // Method to trigger the attack animation
    public override void TriggerAttackAnimation(string attackTrigger)
    {
        // Check if the zombie is alive and the animator component is not null
        if (isAlive && animator != null)
        {
            LookAtPlayer();
            // Set the trigger parameter to initiate the attack animation
            animator.SetTrigger(attackTrigger);
        }
    }
}