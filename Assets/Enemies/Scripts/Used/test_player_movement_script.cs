using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test_player_movement_script : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 5f;
    public float maxHealth = 100f;
    public float playerHealth;
    public float playerDamage = 20f;
    public float dangerDistance = 5f;
    public float attackDelay = 2f;
    public bool isAlive = true;
    private List<GameObject> detectedEnemies = new List<GameObject>();
    public HealthBar healthbar;

    private bool isAttacked = false;

    void Start()
    {
        playerHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
    }

    void Update()
{
    // Debug.Log("Update is called from the player script");
    if (playerHealth > 0f)
    {
        HandleMovementInput();

        // Check for nearby enemies using Physics
        if (CheckForNearbyEnemies())
        {
            HandleDanger();
        }
    }
}

void HandleDanger()
{
    // Apply damage over time if an enemy is detected and the player is being attacked
    TakeDamageOverTime();
}

void TakeDamageOverTime()
{
    playerHealth -= Time.deltaTime * 10f;
    healthbar.SetHealth(playerHealth);
    if (playerHealth <= 0)
    {
        isAlive = false;
        Die();
    }
}

bool CheckForNearbyEnemies()
{
    Collider[] colliders = Physics.OverlapSphere(transform.position, dangerDistance);

    foreach (var collider in colliders)
    {
        // Check if the collider has the "Enemy" tag
        if (collider.CompareTag("Enemy"))
        {
            BaseUniversal enemyScript = collider.gameObject.GetComponent<BaseUniversal>();

            // Check if the enemy has the BaseUniversal component and is alive
            if (enemyScript != null && enemyScript.IsAlive())
            {
                // Calculate the distance to the enemy here
                float distanceToEnemy = Vector3.Distance(transform.position, enemyScript.transform.position);

                // Play the attack animation
                enemyScript.UpdateAnimatorParameters();
                enemyScript.TriggerAttackAnimation("BiteTrigger");

                // Apply damage if the player is still alive and the enemy is within range
                if (IsAlive() && distanceToEnemy <= enemyScript.AttackRange)
                {
                    TakeDamage(enemyScript.damage);
                }

                return true; // Player is in danger
            }
        }
    }

    return false; // Player is not in danger
}


    IEnumerator WaitForAttackAnimation(BaseUniversal enemyScript, float distanceToEnemy)
    {
        // Wait for the attack animation to finish
        yield return new WaitForSeconds(attackDelay);

        // Check if the player is still alive, the enemy is within range, and the player is being attacked
        if (IsAlive() && distanceToEnemy <= enemyScript.AttackRange && isAttacked)
        {
            // Apply damage
            TakeDamageOverTime();
        }

        // Reset the attacked flag
        isAttacked = false;
    }

    public void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;
        transform.Translate(movement * speed * Time.deltaTime);
    }

    public void HandleAttack()
    {
        Debug.Log("HandleAttack");
        if (playerHealth > 0f)
        {
            TakeDamage(playerDamage);
        }
    }

    public void TakeDamage(float damage)
    {
        playerHealth -= damage;

        playerHealth = Mathf.Max(playerHealth, 0f);

        healthbar.SetHealth(playerHealth);

        Debug.Log(playerHealth);
        if (playerHealth <= 0)
        {
            isAlive = false;
            Die();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            BaseUniversal enemyScript = collision.gameObject.GetComponent<BaseUniversal>();
            if (enemyScript != null && enemyScript.IsAlive())
            {
                enemyScript.TakeDamage(playerDamage);
            }
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
