using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    public int lastCompletedLevel = 0;
    public int score = 0;
    public int gold = 0;
    public int healthIncreaseLevel = 0;
    public int damageBoostLevel = 0;
    public float maxHealth = 100f;
}