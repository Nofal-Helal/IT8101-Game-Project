using UnityEngine;

public class ZombieScript : BaseUniversal
{
    public float zombieSpeed = 2f;
    public float playerProximityDistance = 5f;
    public bool isPlayerCloseLogSent;
    public float attackDelay = 2f;
    //public Animator animator;

    private new void Start()
    {
        isPlayerCloseLogSent = false;
        animator = GetComponent<Animator>(); // Add this line
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

    protected override void MoveTowardsPlayer(Vector3 playerPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, playerPosition, zombieSpeed * Time.deltaTime);
        transform.LookAt(playerPosition);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    protected override void AttackPlayer(test_player_movement_script playerScript)
    {
        if (timeSinceLastAttack >= attackCooldown)
        {
            base.AttackPlayer(playerScript);
            timeSinceLastAttack = 0f;
            Invoke("ResetAttack", attackDelay);
        }
    }
    protected void ResetAttack()
    {
        isPlayerCloseLogSent = false;
        UpdateAnimatorParameters();
        TriggerRunAnimation();
    }

    protected override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        base.Die();
    }

    private GameObject FindPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    private void HandlePlayerProximity(GameObject player)
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
                    TriggerBiteAnimation();
                }
                else
                {
                    // Log message only once when the player becomes close
                    Debug.Log("Player is close to the zombie! Attacking...");
                    isPlayerCloseLogSent = true;
                    UpdateAnimatorParameters();  // Call this when the player is close
                    TriggerRunAnimation();   // Call this when the player is close
                }
            }

            MoveTowardsPlayer(player.transform.position);
            // Implement attack logic
            AttackPlayer(player.GetComponent<test_player_movement_script>());

        }
        else
        {
            // Player is not close
            if (isPlayerCloseLogSent)
            {
                // Log message only once when the player is not close
                Debug.Log("Player is not close to the zombie. Going into idle.");
                isPlayerCloseLogSent = false;
                UpdateAnimatorParameters();  // Call this when the player is not close
                TriggerIdleAnimation();
            }
        }
    }
    private void UpdateAnimatorParameters()
    {
        animator.SetBool("isPlayerCloseLogSent", isPlayerCloseLogSent);
        animator.SetBool("BiteTrigger", isPlayerCloseLogSent);

    }

    private void TriggerRunAnimation()
    {
        animator.SetTrigger("RunTrigger"); // Replace with your actual trigger name
    }

    private void TriggerIdleAnimation()
    {
        animator.SetTrigger("IdleTrigger"); // Replace with your actual trigger name
    }
    private void TriggerBiteAnimation()
    {
        animator.SetTrigger("BiteTrigger");
    }
}
