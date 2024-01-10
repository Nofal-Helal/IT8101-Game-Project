using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HealthUIManager : MonoBehaviour
{
    public TextMeshProUGUI playerHealth;
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        if (playerHealth == null)
        {
            Debug.Log("You don't have the gold text attached");
            return;
        }
        player = FindObjectOfType<Player>();

        if (player == null)
        {
            Debug.Log("Player couldn't be found");
            return;
        }
    }
    // Update is called once per frame
    void Update()
    {
        playerHealth.text = player.health.ToString();
    }
}
