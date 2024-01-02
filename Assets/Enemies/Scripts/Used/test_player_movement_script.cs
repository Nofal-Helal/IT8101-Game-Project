using System.Collections;
using UnityEngine;
using YourGameNamespace;

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

    // Flag to track if the player is in contact with an enemy
    private bool isPlayerInContactWithEnemy = false;

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
        }
    }

    // Apply damage over time when in contact with an enemy
    void TakeDamageOverTime(float damage)
    {
        currentHealth -= Time.deltaTime * damage;
        currentHealth = Mathf.Max(currentHealth, 0f);
        healthbar.SetHealth(currentHealth);
        UpdateHUD();

        if (currentHealth <= 0)
        {
            isAlive = false;
            Die();
        }
    }

    // OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isPlayerInContactWithEnemy = true;
            BaseUniversal enemyScript = collision.gameObject.GetComponent<BaseUniversal>();

            // Apply damage over time when in contact with an enemy
            TakeDamageOverTime(enemyScript.damage);
        }
    }

    // OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isPlayerInContactWithEnemy = false;
        }
    }

    // Coroutine to wait for the attack animation and apply damage over time
    IEnumerator WaitForAttackAnimation(BaseUniversal enemyScript, float distanceToEnemy)
    {
        yield return new WaitForSeconds(attackDelay);

        if (IsAlive() && distanceToEnemy <= enemyScript.AttackRange && isAttacked)
        {
            // TakeDamageOverTime(); // You can uncomment this line if needed
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            BaseUniversal enemyScript = collision.gameObject.GetComponent<BaseUniversal>();
            if (enemyScript != null && enemyScript.IsAlive())
            {
                // Call TakeDamage method of the enemy
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
