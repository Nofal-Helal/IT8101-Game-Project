using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WeaponUIManager : MonoBehaviour
{
    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI ammoAmount;
    private GunLoadout weaponLoadout;
    private GunData gunData;
    // Start is called before the first frame update
    void Start()
    {
        if (weaponName == null)
        {
            Debug.Log("You don't have the gold text attached");
            return;
        }

        if (ammoAmount == null)
        {
            Debug.Log("You don't have the score text attached");
            return;
        }

        weaponLoadout = GameObject.FindObjectOfType<GunLoadout>();

        if (weaponLoadout == null)
        {
            Debug.Log("Couldn't find the weapon loadout object");
            return;
        }
    }
    // Update is called once per frame
    void Update()
    {
        gunData = weaponLoadout.currentGun.GetComponent<Gun>() == null ? weaponLoadout.currentGun.GetComponent<Projectile>().gunData : weaponLoadout.currentGun.GetComponent<Gun>().gunData;

        weaponName.text = gunData.name;
        ammoAmount.text = "Ammo: " + gunData.magazine.ToString();
    }
}
