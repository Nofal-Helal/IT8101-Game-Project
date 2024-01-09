using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GunData gunData;
    public event Action OnThrow;
    void Start()
    {
        gunData.reloading = false;
        gunData.shooting = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !gunData.shooting)
        {
            StartCoroutine(Throw());
        }
    }
    public IEnumerator Throw()
    {
        Debug.Log("Throwing");
        gunData.shooting = true;
        OnThrow?.Invoke();
        gameObject.transform.localScale = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(1f / gunData.fireRate);
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        gunData.shooting = false;
    }
    public static float GetDamageValue(float distance, GunData gunData)
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
