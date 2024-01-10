using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsData", menuName = "SettingsData")]
public class SettingsData : ScriptableObject
{
    public float masterVolume = 0.5f,
        cartVolume = 0.5f,
        weaponsVolume = 0.5f,
        monstersVolume = 0.5f;

    public string inputBindingsJson = "";
}
