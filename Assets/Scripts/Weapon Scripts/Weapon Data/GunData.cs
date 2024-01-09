using UnityEngine;
// using UnityEditor.Presets;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapon/Gun")]
public class GunData : ScriptableObject
{
  public new string name;
  public WeaponType weaponType;
  public int maxAmmo;
  public int cost;
  public float reloadTime;
  public float fireRate;
  [Header("Range Info")]
  public float shortRangeDistance;
  public int shortRangeDamage;
  public int midRangeDistance;
  public int midRangeDamage;
  public int longRangeDistance;
  public int longRangeDamage;
  [Header("In Game Data")]
  // public Preset gunIdlePosition;
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