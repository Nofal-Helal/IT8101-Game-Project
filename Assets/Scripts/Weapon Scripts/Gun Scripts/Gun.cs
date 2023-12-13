using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class Gun : MonoBehaviour, IGun
{
    public GunData gunData;
    public event Action OnShoot;
    public event Action OnReload;
    void Start()
    {
        gunData.reloading = false;
        gunData.shooting = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !gunData.shooting && !gunData.reloading)
        {
            StartCoroutine(Shoot());
        }

        if ((Input.GetKeyDown(KeyCode.R) || gunData.magazine == 0) && !gunData.reloading)
        {
            StartCoroutine(Reload());
        }
    }
    public IEnumerator Shoot()
    {
        Debug.Log("Shooting");
        gunData.shooting = true;
        OnShoot?.Invoke();
        gunData.magazine -= 1;
        yield return new WaitForSeconds(1f / gunData.fireRate);
        gunData.shooting = false;
    }
    public IEnumerator Reload()
    {
        Debug.Log("Reloading");
        gunData.reloading = true;
        OnReload?.Invoke();
        yield return new WaitForSeconds(gunData.reloadTime);
        Debug.Log("Done reloading");
        gunData.magazine = gunData.maxAmmo;
        gunData.reloading = false;
    }
}
