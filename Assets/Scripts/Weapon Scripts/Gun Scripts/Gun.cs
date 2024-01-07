using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class Gun : MonoBehaviour, IGun
{
    private FirstPersonCamera firstPersonCamera;
    public GunData gunData;
    public event Action OnShoot;
    public event Action OnReload;
    GameInputActions.GameplayActions inputActions;
    void Start()
    {
        gunData.reloading = false;
        gunData.shooting = false;
        firstPersonCamera = FindObjectOfType<FirstPersonCamera>();
        inputActions = Global.inputActions.gameplay;
    }
    // Update is called once per frame
    void Update()
    {
        if (!firstPersonCamera.acceptingInput)
        {
            return;
        }

        if (inputActions.Shoot.IsPressed() && !gunData.shooting && !gunData.reloading)
        {
            StartCoroutine(Shoot());
        }

        if ((inputActions.Reload.IsPressed() || gunData.magazine == 0) && !gunData.reloading)
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
    public float GetDamageValue(float distance)
    {
        if (distance >= gunData.longRangeDistance)
        {
            return gunData.longRangeDamage;
        }
        if (distance <= gunData.shortRangeDistance)
        {
            return gunData.shortRangeDamage;
        }
        return gunData.midRangeDamage;
    }
}
