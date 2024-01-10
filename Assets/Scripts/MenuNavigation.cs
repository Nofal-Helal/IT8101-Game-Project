using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class MenuNavigation : MonoBehaviour
{
    void OnEnable()
    {
        Global.settingsData = (SettingsData)Resources.Load("SettingsData");
        Global.playerData = (PlayerData)Resources.Load("PlayerData");
        Assert.IsNotNull(Global.playerData);
        Assert.IsNotNull(Global.settingsData);
        if (!string.IsNullOrEmpty(Global.settingsData.inputBindingsJson))
        {
            Global.inputActions.LoadBindingOverridesFromJson(Global.settingsData.inputBindingsJson);
        }
    }

    public void goToScene(string sceneName)
    {
        SceneTransition.Fade(sceneName);
    }
    public void showSomething(string InputValue)
    {

    }

    public void NewGame()
    {
        SceneTransition.Fade("Scenes/Level_1/Level_1");
    }

    public void LoadGame()
    {
        string scene = Global.playerData.lastCompletedLevel switch
        {
            0 => "Scenes/Level_1/Level_1",
            1 => "Scenes/Level_2/Level 2",
            2 => "Scenes/Level_3/Level3",
            3 => "Scenes/Level 4/Level 4",
            _ => throw new System.Exception("Attempt to access secret level")
        };

        SceneTransition.Fade(scene);
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
