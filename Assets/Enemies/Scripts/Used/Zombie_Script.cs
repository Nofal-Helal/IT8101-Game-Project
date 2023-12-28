using UnityEngine;

public class ZombieScript : BaseUniversal
{
    public float zombieSpeed = 2f;
    public float playerProximityDistance = 5f;
    private bool isPlayerCloseLogSent = false;
    //public Animator animator;


    private void Start()
    {

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
        }
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
                // Log message only once when the player becomes close
                Debug.Log("Player is close to the zombie! Attacking...");
                isPlayerCloseLogSent = true;
            }
            MoveTowardsPlayer(player.transform.position);

            // Implement attack logic
            AttackPlayer(player.GetComponent<test_player_movement_script>());
            // UpdateAnimatorParameters();
            // TriggerAttackAnimation();
        }
        else
        {
            // Player is not close
            if (isPlayerCloseLogSent)
            {
                // Log message only once when the player is not close
                Debug.Log("Player is not close to the zombie. Going into idle.");
                isPlayerCloseLogSent = false;
            }

        }
    }

    // private void UpdateAnimatorParameters()
    // {
    //     animator.SetBool("isPlayerCloseLogSent", isPlayerClose);
    // }

    // private void TriggerAttackAnimation()
    // {
    //     animator.SetTrigger("Attack 1"); // Replace with your actual trigger name
    // }
}