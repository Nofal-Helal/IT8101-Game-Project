using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EconomyUIManager : MonoBehaviour
{
    public TextMeshProUGUI goldAmount;
    public TextMeshProUGUI scoreAmount;
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        if (goldAmount == null)
        {
            Debug.Log("You don't have the gold text attached");
            return;
        }

        if (scoreAmount == null)
        {
            Debug.Log("You don't have the score text attached");
            return;
        }
        player = FindObjectOfType<Player>();

        if (player == null)
        {
            Debug.Log("Couldn't find a player object");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        goldAmount.text = "Gold: " + player.gold;
        scoreAmount.text = "Score: " + player.score;
    }
}
