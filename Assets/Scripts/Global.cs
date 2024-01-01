using UnityEngine;

public static class Global
{
    public static KeyCode shootInput = KeyCode.Mouse0;
    public static KeyCode reloadInput = KeyCode.R;
    public static GameInputActions inputActions = new();
    public static float masterVolume = 0.5f,
        cartVolume = 0.5f,
        weaponsVolume = 0.5f,
        monstersVolume = 0.5f;
}
