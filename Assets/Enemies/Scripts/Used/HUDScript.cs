using UnityEngine;
using TMPro;

public class HUDScript : MonoBehaviour
{
    public TextMeshProUGUI healthText;

    private int playerHealth;

    public void Initialize(int initialHealth)
    {
        // Set the initial player health and update the health display
        playerHealth = initialHealth;
        UpdateHealth(playerHealth);
    }

    public void UpdateHealth(int newHealth)
    {
        // Update the player health and the displayed health value
        playerHealth = newHealth;
        healthText.text = "Health: " + playerHealth.ToString();
    }
}
