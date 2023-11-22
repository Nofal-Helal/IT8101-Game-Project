using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    private GunLoadout loadout;
    private TextMeshProUGUI[] hudElements;
    private GameObject currentWeapon;
    // Start is called before the first frame update
    void Start()
    {
        loadout = GameObject.FindFirstObjectByType<GunLoadout>();
        hudElements = GameObject.FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);

    }

    // Update is called once per frame
    void Update()
    {
        if (loadout.gunList.Count > 0)
        {
            currentWeapon = loadout.gunList[loadout.weaponIndex];
            hudElements[0].text = "Ammo: " + currentWeapon.GetComponent<Gun>().gunData.magazine;
        }
    }
}
