using System;
using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour, IGun
{
    public GunData gunData;
    public event Action OnShoot;
    public event Action OnReload;
    GameInputActions.GameplayActions inputActions;
    void Start()
    {
        gunData.reloading = false;
        gunData.shooting = false;
        inputActions = Global.inputActions.gameplay;
    }
    // Update is called once per frame
    void Update()
    {
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

        bool hit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitInfo, 100f, 1 << 8);
        if (hit) DealDamage(hitInfo);
        if (hit) Debug.Log("Dist: " + hitInfo.distance);

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

    public void DealDamage(RaycastHit hitInfo)
    {
        float damage = hitInfo.distance switch
        {
            float distance when distance >= 22f => gunData.longRange,
            float distance when distance >= 11f => gunData.midRange,
            float distance when distance >= 0f => gunData.shortRange,
            _ => 0
        };

        // TODO: make work with all enemies
        if (hitInfo.rigidbody.TryGetComponent(out Moleman moleman))
        {
            moleman.TakeDamage(damage);
        }

    }
}
