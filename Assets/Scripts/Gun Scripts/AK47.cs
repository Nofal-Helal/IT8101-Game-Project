using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AK74 : MonoBehaviour, IGun
{
    [SerializeField] GunData gunData;

    void Start()
    {
        gunData.timeSinceLastShot = 0;
        gunData.magazine = 30;
        gunData.baseDamage = 10;
        gunData.fireRate = 3.5f;

    }
    // Update is called once per frame
    void Update()
    {
        if (gunData.timeSinceLastShot <= (1.0f / gunData.fireRate))
        {
            gunData.timeSinceLastShot += Time.deltaTime;
        }

        if (Input.GetMouseButton(0))
        {
            Shoot();
        }

        if ((Input.GetKeyDown(KeyCode.R) || gunData.magazine == 0) && !gunData.reloading)
        {
            StartCoroutine(Reload());
        }
    }
    public void Shoot()
    {
        if (gunData.timeSinceLastShot <= (1.0f / gunData.fireRate) || gunData.reloading)
        {
            return;
        }
        Debug.Log("Shooting");
        gunData.magazine -= 1;
        gunData.timeSinceLastShot = 0;
        Debug.Log(gunData.magazine);
    }
    public IEnumerator Reload()
    {
        Debug.Log("Reloading");
        gunData.reloading = true;
        yield return new WaitForSeconds(gunData.reloadTime);
        Debug.Log("Done reloading");
        gunData.magazine = 30;  // Reset ammo after reloading
        gunData.reloading = false;
    }
}
