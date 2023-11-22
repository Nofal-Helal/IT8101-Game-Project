using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour, IGun
{
    public GunData gunData;
    // private Animator animator;
    void Start()
    {
        gunData.reloading = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (gunData.timeSinceLastShot <= (1f / gunData.fireRate))
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
        gunData.magazine = gunData.maxAmmo;
        gunData.reloading = false;
    }
}
