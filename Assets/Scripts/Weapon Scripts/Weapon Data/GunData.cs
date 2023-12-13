using UnityEngine;
using System;
using System.Collections;
using UnityEditor.Presets;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapon/Gun")]
public class GunData : ScriptableObject
{
  public new string name;
  public WeaponType weaponType;
  public int maxAmmo;
  public float reloadTime;
  public float fireRate;
  [Header("Damage")]
  public int shortRange;
  public int midRange;
  public int longRange;
  [Header("In Game Data")]
  public Preset gunIdlePosition;
  [HideInInspector]
  public bool reloading;
  [HideInInspector]
  public bool shooting;
  [HideInInspector]
  public int magazine;
}

public enum WeaponType
{
  PRIMARY,
  SECONDARY,
  PROJECTILE
}