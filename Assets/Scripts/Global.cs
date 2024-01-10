using UnityEngine;

public static class Global
{
    public static GameInputActions inputActions = new();

    public static SettingsData settingsData = (SettingsData)Resources.Load("SettingsData");
    public static PlayerData playerData = (PlayerData)Resources.Load("PlayerData");

    public static float masterVolume => settingsData.masterVolume;
    public static float cartVolume => settingsData.cartVolume;
    public static float weaponsVolume => settingsData.weaponsVolume;
    public static float monstersVolume => settingsData.monstersVolume;

    public static Color transitionColor = new Color(0x17 / 255f, 0x18 / 255f, 0x18 / 255f);
    public static float transitionSpeed = 2f;

    // score and gold before failing a level
    public static int lastScore;
    public static int lastGold;
}
