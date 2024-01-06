using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class HUD : MonoBehaviour
{
    private GunLoadout loadout;
    private TextMeshProUGUI[] hudElements;
    private GameObject currentWeapon;
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        loadout = GameObject.FindFirstObjectByType<GunLoadout>();
        player=GameObject.FindFirstObjectByType<Player>();
        hudElements = transform.GetComponentsInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
            currentWeapon = loadout.gunList[loadout.weaponIndex];
        if (loadout.gunList.Count > 0)
        {
            hudElements[0].text = "Ammo: " + currentWeapon.GetComponent<Gun>().gunData.magazine;
            hudElements[1].text = "Health: " + player.health;
        }
    }
}