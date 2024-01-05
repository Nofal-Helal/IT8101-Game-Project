using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class BatEnemyScript : BaseUniversal
{
    // Proximity distance to detect the player
    public float playerProximityDistance;

    // Delay before attacking after detecting the player
    public float attackDelay = 2f;

    // Duration of the attack animation
    public float attackAnimationLength = 1.5f;

    // Flag to check if the bat is colliding with the player
    private bool isCollidingWithPlayer = false;

    // Height at which the bat hovers
    private float hoverHeight = 5f;

    // Maximum height the bat can reach
    private float maxHeight = 10f;

    // Force applied to keep the bat hovering
    public float hoverForce = 10f;

    // Rigidbody component of the bat
    private Rigidbody rb;

    // Initialization
    private new void Start()
    {
        base.Start();
        isPlayerCloseLogSent = false;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Check if the Animator component is present
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the object or its children.");
        }

        // Set rigidbody properties
        if (rb != null)
        {
            rb.mass = 1f;
            rb.drag = 0.5f;
            rb.angularDrag = 0.5f;
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
        if (isAlive)
        {
            HoverInAir();
        }
    }

    // Apply a constant force upward to keep the bat hovering
    private void HoverInAir()
    {
        if (rb != null)
        {
            rb.AddForce(Vector3.up * hoverForce, ForceMode.Force);

            // Limit the height of the bat
            Vector3 clampedPosition = transform.position;
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, hoverHeight, maxHeight);
            transform.position = clampedPosition;
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

            // Call DealDamage with the damage parameter
            DealDamage(damage);
        }
    }

    // Coroutine to wait for the attackDelay before initiating the attack
    IEnumerator WaitForAttack(Player playerScript)
    {
        yield return new WaitForSeconds(attackDelay);

        // Check if still attacking and player is within attack range and colliding
        if (isAttacking && IsPlayerInRange(playerScript.transform.position) && isCollidingWithPlayer)
        {
            // Player is in range, initiate attack
            TriggerAttackAnimation("AttackTrigger");
            Debug.Log("Attack animation triggered");

            // Wait for the duration of the attack animation
            yield return new WaitForSeconds(attackAnimationLength);

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
        TriggerRunAnimation();
    }

    // Finish the attack
    public override void FinishAttack()
    {
        Debug.Log("Finished attacking, time to fly away");
        isPlayerCloseLogSent = false;
        isAttacking = false;
        UpdateAnimatorParameters();
        animator.ResetTrigger("AttackTrigger");
        TriggerRunAnimation();
    }

    // Take damage
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    // Handle death
    protected override void Die()
    {
        if (health <= 0 && isAlive)
        {
            TriggerDeathAnimation("DeathTrigger");
            Debug.Log("OOF");
            isAlive = false;
            rb.useGravity = true; // Enable gravity to let the bat fall
            //Base Death Method
            base.Die();
        }
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
                    Debug.Log("Player is close to the bat! Flying towards...");
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
                    // Player is not close enough for attack, keep flying
                    TriggerRunAnimation();
                }
            }
            else
            {
                if (isPlayerCloseLogSent)
                {
                    Debug.Log("Player is not close to the bat. Going into idle.");
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
                // Player collided with the bat, initiate attack
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
        animator.SetTrigger("FlyTrigger");
    }

    // Trigger the idle animation
    public override void TriggerIdleAnimation(string triggerName)
    {
        animator.SetTrigger("IdleTrigger");
    }
}
