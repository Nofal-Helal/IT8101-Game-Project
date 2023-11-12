using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class GunData
{
  public int magazine;
  public int baseDamage;
  public float reloadTime;
  public float fireRate;
  [HideInInspector]
  public float timeSinceLastShot;
  [HideInInspector]
  public bool reloading;
}