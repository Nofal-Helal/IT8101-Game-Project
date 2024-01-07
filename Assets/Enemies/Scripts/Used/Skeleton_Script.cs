using NaughtyAttributes;
using System.Collections;
using UnityEngine;

// SkeletonScript inherits from the BaseUniversal class
public class SkeletonScript : BaseUniversal
{
    // Public variables exposed in the Unity Editor
    // public float playerProximityDistance;
    public float attackDelay = 2f;
    public float attackAnimationLength = 1.5f;
    private bool isCollidingWithPlayer = false;

    // Start is called before the first frame update
    private new void Start()
    {
        // Call the Start method of the base class
        base.Start();
        base.goldDropAmount = 50;
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
        // Call the Update method of the base class
        base.Update();
        if (isAlive)
        {
            GameObject player = FindPlayer();
            if (player != null)
            {
                // Check and handle player proximity
                HandlePlayerProximity(player);
            }
        }
    }

    // Method to handle player attack
    protected override void AttackPlayer(Player playerScript)
    {
        if (timeSinceLastAttack >= attackCooldown)
        {
            // Call the AttackPlayer method of the base class
            base.AttackPlayer(playerScript);
            timeSinceLastAttack = 0f;
            animator.SetTrigger("AttackTrigger");

            // Apply damage after attackDelay
            StartCoroutine(WaitForAttack(playerScript));
        }
    }

    // Coroutine to wait for the attack to occur
    IEnumerator WaitForAttack(Player playerScript)
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

    // Method to reset the attack state
    protected void ResetAttack()
    {
        Debug.Log("ResetAttack");
        isAttacking = false;
        isPlayerCloseLogSent = false;
        isCollidingWithPlayer = false;
        UpdateAnimatorParameters();
        TriggerRunAnimation("WalkTrigger");
    }

    // Method to finish the attack
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

    // Method to handle death
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

    // Method to find the player in the scene
    private GameObject FindPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    // Method to handle player proximity
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
                    StartCoroutine(WaitForAttack(player.GetComponent<Player>()));
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

    // Method called when a collision occurs
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player playerScript = collision.gameObject.GetComponent<Player>();
            if (playerScript != null && playerScript.isAlive())
            {
                // Player collided with the skeleton, initiate attack
                isAttacking = true;
                isCollidingWithPlayer = true;
                AttackPlayer(playerScript);
            }
        }
    }

    // Method to deal damage to the player
    public override void DealDamage(float damage)
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

    // Method to update animator parameters
    public override void UpdateAnimatorParameters()
    {
        if (animator != null)
        {
            animator.SetBool("isPlayerCloseLogSent", isPlayerCloseLogSent);
            animator.SetBool("AttackTrigger", isAttacking);
        }
    }

    // Method to trigger run animation
    private void TriggerRunAnimation()
    {
        animator.ResetTrigger("AttackTrigger"); // Reset the AttackTrigger
        animator.SetTrigger("WalkTrigger");
    }

    // Method to trigger idle animation
    public override void TriggerIdleAnimation(string triggerName)
    {
        animator.SetTrigger("IdleTrigger");
    }
}