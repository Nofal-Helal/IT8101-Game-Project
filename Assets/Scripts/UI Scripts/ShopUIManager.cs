using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    private Player player;
    private GunLoadout gunLoadout;
    private Shop shop;
    public TextMeshProUGUI goldAmountText;
    public Button AK47Button;
    public Button PistolButton;
    public Button GrenadeButton;
    public Button DamageBoostButton;
    public Button HealthIncreaseButton;
    GameInputActions.GameplayActions inputActions;
    // Start is called before the first frame update
    void Start()
    {
        gunLoadout = FindObjectOfType<GunLoadout>();
        player = FindObjectOfType<Player>();
        shop = FindObjectOfType<Shop>();

        gameObject.SetActive(false);
    }

    void Update()
    {
        goldAmountText.text = "Gold: " + player.playerData.gold.ToString();
        DamageBoostButton.GetComponentInChildren<TextMeshProUGUI>().text = "Damage Boost: " + "Level " + (player.playerData.damageBoostLevel + 1) + ", " + shop.damageBoostCost + " Gold Ingots";
        HealthIncreaseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Health Increase: " + "Level " + (player.playerData.healthIncreaseLevel + 1) + ", " + shop.healthBoostCost + " Gold Ingots";
    }

    public void Buy(GameObject weapon)
    {
        GunData weaponData = weapon.GetComponent<Gun>() != null ? weapon.GetComponent<Gun>().gunData : weapon.GetComponent<Projectile>().gunData;
        if (player.playerData.gold - weaponData.cost < 0)
        {
            Debug.Log("Not enough money!");
            return;
        }
        gunLoadout.AddToLoadout(weapon);
        Debug.Log("bought something :D");
    }
    public void IncreaseHealth()
    {
        if (player.playerData.gold - shop.healthBoostCost < 0)
        {
            Debug.Log("Not enough money!");
            // Alert the user
            return;
        }

        if (player.playerData.healthIncreaseLevel + 1 > 3)
        {
            Debug.Log("You reached the maximum level");
            // Alert the user
            return;
        }
        player.playerData.gold -= shop.healthBoostCost;
        player.playerData.healthIncreaseLevel += 1;
        player.IncreaseMaxHealth();
    }
    public void BoostDamage()
    {
        if (player.playerData.gold - shop.damageBoostCost < 0)
        {
            Debug.Log("Not enough money!");
            // Alert the user
            return;
        }

        if (player.playerData.damageBoostLevel + 1 > 3)
        {
            Debug.Log("You reached the maximum level");
            // Alert the user
            return;
        }
        player.playerData.gold -= shop.damageBoostCost;
        player.playerData.damageBoostLevel += 1;
    }
}
