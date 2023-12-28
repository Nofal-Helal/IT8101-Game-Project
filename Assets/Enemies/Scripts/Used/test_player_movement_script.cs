using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test_player_movement_script : MonoBehaviour
{
    // Start is called before the first frame update

    public float speed = 5f;
    public float mouseSensitivity = 5f;
    public float maxHealth = 100f;
    public float playerHealth;
    public float dangerDistance= 5f;

    public HealthBar healthbar;


    void Start()
    {
        playerHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
    }
    // Update is called once per frame
    void Update()
    {
        if (playerHealth > 0f)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;
            transform.Translate(movement * speed * Time.deltaTime);

            // Check for nearby enemies using Physics
            bool isInDanger = CheckForNearbyEnemies();
            
            if (isInDanger)
            {
                // Additional logic when the player is in danger
                playerHealth -= Time.deltaTime * 10f;
                healthbar.SetHealth(playerHealth);
            }
        }
    }

    bool CheckForNearbyEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, dangerDistance);
        
        foreach (var collider in colliders)
        {
            // Check if the collider belongs to an enemy
            if (collider.CompareTag("Enemy"))
            {
                return true; // Player is in danger
            }
        }

        return false; // Player is not in danger
    }

    public void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;
        transform.Translate(movement * speed * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        playerHealth -= damage;

        playerHealth = Mathf.Max(playerHealth, 0f);

        healthbar.SetHealth(playerHealth);

        Debug.Log(playerHealth);
        if (playerHealth <= 0)
        {
            Die();

        }

    }

    public void Die()
    {
        Destroy(gameObject);
    }


}
