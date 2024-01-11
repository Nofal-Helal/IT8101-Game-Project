using System.Collections;
using UnityEngine;

public class BaseUniversal : MonoBehaviour, IDamageTaker
{
    // Basic attributes
    public bool isAlive = true;
    public float health = 100f;
    public float speed = 1f;
    public float damage;
    public float attackRange = 1f;
    public float attackSpeed = 1f;
    public float attackCooldown = 1f;
    public float timeSinceLastAttack;
    public bool isActivated = false;
    public bool isAttacking = true;
    // Gold is the currency you get when you kill an enemy.
    public int goldDropAmount = 20;
    // A "score" is a value you get when you hit an enemy.
    public int scoreAmount = 5;
    // References
    protected Player player;
    protected FirstPersonCamera playerCamera;
    protected Animator animator;
    public bool isPlayerCloseLogSent = false;
    public float playerProximityDistance;
    protected float originalAttackRange;

    // Initialization
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animation component not found");
        }
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        originalAttackRange = attackRange;
        playerCamera = FindObjectOfType<FirstPersonCamera>();
    }

    // Update logic
    protected virtual void Update()
    {
        if (!isActivated)
        {
            Debug.Log("Not Activated");
            return;
        }

        if (isAttacking)
        {
            // Check if the player is close and is in attack range
            HandlePlayerProximity();

            if (IsPlayerInRange(playerCamera.transform.position))
            {
                Debug.Log("Hello, I'm supposed to kill ya!");
                // Apply damage to the player
                ((IDamageTaker)player).TakeDamage(damage * Time.deltaTime);
            }

            if (animator != null)
            {
                animator.Play("Zombie|ZombieBite");
            }
        }

    }
    // Handle player proximity
    public virtual void HandlePlayerProximity()
    {
        if (player.CompareTag("Player"))
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerCamera.transform.position);
            if (distanceToPlayer <= playerProximityDistance)
            {
                if (!isPlayerCloseLogSent)
                {
                    // Check if the player is in front of the enemy
                    Vector3 directionToPlayer = playerCamera.transform.position - transform.position;
                    float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                    if (angleToPlayer < 180f)  // Adjust the angle threshold as needed
                    {
                        // Player is in front of the enemy
                        if (distanceToPlayer <= attackRange)
                        {
                            Debug.Log("Enemy: I'm gonna attack!");
                            isPlayerCloseLogSent = true;
                            UpdateAnimatorParameters();
                            TriggerAttackAnimation("AttackTrigger");
                        }
                        else
                        {
                            Debug.Log("Enemy: Player is close. Going into idle.");
                            isPlayerCloseLogSent = true;
                            UpdateAnimatorParameters();
                            TriggerIdleAnimation("IdleTrigger");
                            attackRange = originalAttackRange;
                        }
                    }
                    else
                    {
                        // Player is not in front of the enemy
                        // Log message only once when the player becomes close
                        Debug.Log("Enemy: Player is close, but not in front. Going into idle.");
                        isPlayerCloseLogSent = true;
                        UpdateAnimatorParameters();
                        TriggerIdleAnimation("IdleTrigger");
                        attackRange = originalAttackRange;
                    }
                }
                MoveTowardsPlayer(playerCamera.transform.position);
            }
            else
            {
                // Player is not close
                if (isPlayerCloseLogSent)
                {
                    // Log message only once when the player is not close
                    Debug.Log("Enemy: Player is not close. Going into idle.");
                    isPlayerCloseLogSent = false;
                    UpdateAnimatorParameters();
                    TriggerIdleAnimation("IdleTrigger");
                }
            }
        }
        else
        {
            isPlayerCloseLogSent = false;
            UpdateAnimatorParameters();
            TriggerIdleAnimation("IdleTrigger");
        }
    }

    // Move towards the player
    public virtual void MoveTowardsPlayer(Vector3 playerPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, playerPosition, speed * Time.deltaTime);
        transform.LookAt(playerPosition);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    // Attack the player
    protected virtual void AttackPlayer(Player playerScript)
    {
        if (isAlive && playerScript != null && CanAttack())
        {
            if (playerScript.isAlive)
            {
                timeSinceLastAttack = 0f;
                isPlayerCloseLogSent = true;
                UpdateAnimatorParameters();
                TriggerAttackAnimation("");

                // Call DealDamage with the damage parameter
                DealDamage(damage);
            }
        }
    }

    // Finish the attack
    public virtual void FinishAttack()
    {
        isAttacking = false;
        isPlayerCloseLogSent = false;
        UpdateAnimatorParameters();
        TriggerRunAnimation("");
    }

    // Check if thisAttackinge enemy can attack
    public bool CanAttack()
    {
        return timeSinceLastAttack >= attackCooldown;
    }

    // Take damage from an external source
    public void TakeDamage(float damage)
    {
        if (!isActivated)
        {
            return;
        }
        health -= damage;
        Debug.Log(health);
        player.playerData.score += scoreAmount;
        Debug.Log(health);
        if (health <= 0)
        {
            Die();
            isAlive = false;
        }
    }

    // Check if the enemy is alive
    public bool IsAlive()
    {
        return isAlive;
    }

    // Update animator parameters
    public virtual void UpdateAnimatorParameters()
    {
        if (animator != null)
        {
            animator.SetBool("isPlayerCloseLogSent", isPlayerCloseLogSent);
        }
    }

    // Trigger the attack animation
    public virtual void TriggerAttackAnimation(string AttackTrigger)
    {
        if (animator != null && isAlive)
        {
            isAttacking = true;
            animator.SetTrigger("AttackTrigger"); // Replace with your actual trigger name
        }
    }

    // Trigger the run animation
    public virtual void TriggerRunAnimation(string RunTrigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(RunTrigger);
        }
    }

    // Trigger the idle animation
    public virtual void TriggerIdleAnimation(string IdleTrigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(IdleTrigger);
        }
    }
    //Trigger the death animation
    public virtual void TriggerDeathAnimation(string DeathTrigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(DeathTrigger);
        }
    }

    // Initiates the die process by marking the entity as not alive and triggering the death animation.
    protected virtual void Die()
    {
        isAlive = false;
        player.playerData.gold += goldDropAmount;
        Debug.Log(player.playerData.gold);
        UpdateAnimatorParameters();
        TriggerDeathAnimation("DeathTrigger");
        HandleDeathAnimationEnd();
    }
    // Handles the end of the death animation by starting a coroutine with a specified delay before executing the actual die logic.
    public virtual void HandleDeathAnimationEnd()
    {
        StartCoroutine(DieAfterDelay(5f));
    }

    // Coroutine: Introduces a delay specified by 'delaySeconds' using WaitForSeconds,
    // and then executes the actual die logic by setting 'isAlive' to false and destroying the game object.
    private IEnumerator DieAfterDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        // Actual die logic after waiting
        Destroy(gameObject);
    }

    // Animation event function called when the attack animation deals damage
    public virtual void DealDamage(float damage)
    {
        GameObject player = FindPlayer();
        if (player != null)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null && playerScript.isAlive && IsPlayerInRange(player.transform.position))
            {
                ((IDamageTaker)playerScript).TakeDamage(damage);
            }
        }
    }
    // Handle collision with player to apply damage
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player playerScript = collision.gameObject.GetComponent<Player>();
            if (playerScript != null && playerScript.isAlive)
            {
                // Call TakeDamage method of the player
                ((IDamageTaker)playerScript).TakeDamage(damage);
            }
        }
    }

    // Check if the player is in attack range
    public virtual bool IsPlayerInRange(Vector3 playerPosition)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);
        return distanceToPlayer <= attackRange;
    }

    // Find the player object in the scene
    private GameObject FindPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }
}