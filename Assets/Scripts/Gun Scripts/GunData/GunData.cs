using UnityEngine;
using System;
using System.Collections;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapon/Gun")]
public class GunData : ScriptableObject
{
  public new string name;
  public int maxAmmo;
  public int baseDamage;
  public float reloadTime;
  public float fireRate;
  [HideInInspector]
  public float timeSinceLastShot;
  [HideInInspector]
  public bool reloading;
  [HideInInspector]
  public int magazine;
}