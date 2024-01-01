using System.Collections;
using UnityEngine;

public class test_player_movement_script : MonoBehaviour
{
    // Player movement parameters
    public float speed = 5f;

    // Player health parameters
    public float maxHealth = 100f;
    private float currentHealth;

    // Player damage parameters
    public float playerDamage = 20f;

    // Danger distance to detect nearby enemies
    public float dangerDistance = 5f;

    // Attack delay for handling damage over time
    public float attackDelay = 2f;

    // Player alive state
    public bool isAlive = true;

    // Health bar UI
    public HealthBar healthbar;

    // Reference to the HUD script
    public HUDScript hudScript;

    // Flag to track if the player is being attacked
    private bool isAttacked = false;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
        hudScript = GameObject.FindObjectOfType<HUDScript>();
        UpdateHUD();  // Call UpdateHUD at the start to set the initial HUD value
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth > 0f)
        {
            HandleMovementInput();

            // Check for nearby enemies using Physics
            if (CheckForNearbyEnemies())
            {
                HandleDanger();
            }
        }
    }

    // Handle danger by applying damage over time
    void HandleDanger()
    {
        TakeDamageOverTime();
    }

    // Apply damage over time
    void TakeDamageOverTime()
    {
        currentHealth -= Time.deltaTime * 10f;
        currentHealth = Mathf.Max(currentHealth, 0f);
        healthbar.SetHealth(currentHealth);
        UpdateHUD();

        if (currentHealth <= 0)
        {
            isAlive = false;
            Die();
        }
    }

    // Check for nearby enemies within the danger distance
    bool CheckForNearbyEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, dangerDistance);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                BaseUniversal enemyScript = collider.gameObject.GetComponent<BaseUniversal>();

                if (enemyScript != null && enemyScript.IsAlive())
                {
                    float distanceToEnemy = Vector3.Distance(transform.position, enemyScript.transform.position);

                    enemyScript.UpdateAnimatorParameters();
                    enemyScript.TriggerAttackAnimation("BiteTrigger");

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

    // Coroutine to wait for the attack animation and apply damage
    IEnumerator WaitForAttackAnimation(BaseUniversal enemyScript, float distanceToEnemy)
    {
        yield return new WaitForSeconds(attackDelay);

        if (IsAlive() && distanceToEnemy <= enemyScript.AttackRange && isAttacked)
        {
            TakeDamageOverTime();
        }

        isAttacked = false;
    }

    // Handle player movement based on input
    public void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;
        transform.Translate(movement * speed * Time.deltaTime);
    }

    // Handle player attack
    public void HandleAttack()
    {
        if (currentHealth > 0f)
        {
            TakeDamage(playerDamage);
        }
    }

    // Apply damage to the player
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);
        healthbar.SetHealth(currentHealth);
        UpdateHUD();

        if (currentHealth <= 0)
        {
            isAlive = false;
            Die();
        }
    }

    // Handle collision with enemy to apply damage
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

    // Handle player death
    public void Die()
    {
        Destroy(gameObject);
    }

    // Check if the player is alive
    public bool IsAlive()
    {
        return isAlive;
    }

    // Update the HUD with current health
    private void UpdateHUD()
    {
        if (hudScript != null)
        {
            hudScript.UpdateHealth((int)currentHealth);
        }
    }
}