using UnityEngine;
using System;

[System.Serializable]
public class GunData
{
  public int maxAmmo;
  public int baseDamage;
  public float reloadTime;
  public float fireRate;
  [HideInInspector]
  public float timeSinceLastShot;
  [HideInInspector]
  public bool reloading;
  [HideInInspector]
  public bool shooting;
}