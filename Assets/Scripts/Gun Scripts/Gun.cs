using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AK47 : MonoBehaviour
{
    [SerializeField] GunData gunData;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        gunData.timeSinceLastShot += Time.deltaTime;

        if (Input.GetMouseButton(0) && gunData.timeSinceLastShot >= 1.0f / gunData.fireRate)
        {
            Debug.Log(gunData.timeSinceLastShot);
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && !gunData.reloading)
        {
            StartCoroutine(Reload());
        }

    }
    void Shoot()
    {
        Debug.Log("Shooting");
        gunData.timeSinceLastShot = 0;
    }
    IEnumerator Reload()
    {
        gunData.reloading = true;
        yield return new WaitForSeconds(gunData.reloadTime);
        gunData.maxAmmo = 30;  // Reset ammo after reloading
        gunData.reloading = false;
    }
}
