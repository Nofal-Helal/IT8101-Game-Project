using System.ComponentModel.Design;
using UnityEngine;

public class BaseUniversal : MonoBehaviour
{
    protected bool isAlive = true;
    public float health = 100f;
    public float speed = 1f;
    public float damage;
    public float attackRange = 1f;
    public float AttackRange => attackRange;
    public float attackSpeed = 1f;
    public float attackCooldown = 1f;
    public float timeSinceLastAttack;
    //protected float rotationSpeed = 5f;
    public bool isAttacking = false;

    public test_player_movement_script player;
    protected Animator animator;
    public bool isPlayerCloseLogSent = false;
    private float playerProximityDistance;
    private float originalAttackRange;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animation component not found");
        }
        originalAttackRange = attackRange;
    }

    protected virtual void Update()
    {
        timeSinceLastAttack += Time.deltaTime;
        if (animator != null)
        {
            animator.SetBool("isAttacking", isAttacking);
        }
    }

    public virtual void HandlePlayerProximity(GameObject player)
    {
        if (player.CompareTag("Player"))
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= playerProximityDistance)
            {
                if (!isPlayerCloseLogSent)
                {
                    // Check if the player is in front of the enemy
                    Vector3 directionToPlayer = player.transform.position - transform.position;
                    float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                    if (angleToPlayer < 45f)  // Adjust the angle threshold as needed
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

                MoveTowardsPlayer(player.transform.position);
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

    public virtual void MoveTowardsPlayer(Vector3 playerPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, playerPosition, speed * Time.deltaTime);
        transform.LookAt(playerPosition);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    protected virtual void AttackPlayer(test_player_movement_script playerScript)
    {
        if (isAlive && playerScript != null && CanAttack())
        {
            if (playerScript.IsAlive())
            {
                timeSinceLastAttack = 0f;
                isPlayerCloseLogSent = true;
                UpdateAnimatorParameters();
                TriggerAttackAnimation("");
            }
        }
    }

    public virtual void FinishAttack()
    {
        isAttacking = false;
        isPlayerCloseLogSent = false;
        UpdateAnimatorParameters();
        TriggerRunAnimation("");
    }

    public bool CanAttack()
    {
        return timeSinceLastAttack >= attackCooldown;
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
            isAlive = false;
        }
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    public virtual void UpdateAnimatorParameters()
    {
        if (animator != null)
        {
            animator.SetBool("isPlayerCloseLogSent", isPlayerCloseLogSent);
        }
    }

    public virtual void TriggerAttackAnimation(string AttackTrigger)
    {
        if (animator != null && isAlive)
        {
            isAttacking = true;
            animator.SetTrigger("AttackTrigger"); // Replace with your actual trigger name
        }
    }

    public virtual void TriggerRunAnimation(string RunTrigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(RunTrigger);
        }
    }

    public virtual void TriggerIdleAnimation(string IdleTrigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(IdleTrigger);
        }
    }

    public virtual void TriggerDeathAnimation(string DeathTrigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(DeathTrigger);
        }
    }

    protected virtual void Die()
    {
        isAlive = false;
        Destroy(gameObject);
    }

    // Animation event function called when the attack animation deals damage
    public virtual void DealDamage()
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
    public virtual bool IsPlayerInRange(Vector3 playerPosition)
    {
        // Implement the logic to check if the player is in range
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);
        return distanceToPlayer <= attackRange;
    }
    private GameObject FindPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }
}
