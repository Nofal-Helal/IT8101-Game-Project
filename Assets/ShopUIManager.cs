using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUIManager : MonoBehaviour
{
    private Player player;
    private Shopkeeper shopkeeper;
    private GunLoadout gunLoadout;
    GameInputActions.GameplayActions inputActions;
    // Start is called before the first frame update
    void Start()
    {
        gunLoadout = FindObjectOfType<GunLoadout>();
        player = FindObjectOfType<Player>();
        shopkeeper = FindObjectOfType<Shopkeeper>();
        gameObject.SetActive(false);
    }

    public void Buy(string weaponName)
    {
        gunLoadout.AddToLoadout(weaponName);
        Debug.Log("bought something :D");
    }
    public void IncreaseHealth()
    {
        if (player.healthIncreaseLevel++ > 3)
        {
            // Alert the user
            return;
        }
        player.healthIncreaseLevel++;
    }
    public void BoostDamage()
    {
        if (player.damageBoostLevel++ > 3)
        {
            // Alert the user
            return;
        }
        player.damageBoostLevel++;
    }
}
