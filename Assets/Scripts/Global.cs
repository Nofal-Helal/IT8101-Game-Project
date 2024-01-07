using UnityEngine;

public static class Global
{
    public static KeyCode shootInput = KeyCode.Mouse0;
    public static KeyCode reloadInput = KeyCode.R;
    public static KeyCode interactInput = KeyCode.E;
    public static KeyCode removeObstacleInput = KeyCode.O;
    public static KeyCode closeMenu = KeyCode.B;
    public static GameInputActions inputActions = new();

    public static float masterVolume = 0.5f,
        cartVolume = 0.5f,
        weaponsVolume = 0.5f,
        monstersVolume = 0.5f;

    public static Color transitionColor = new Color(0x17 / 255f, 0x18 / 255f, 0x18 / 255f);
    public static float transitionSpeed = 2f;
    public static int currentLevelScore;
}
