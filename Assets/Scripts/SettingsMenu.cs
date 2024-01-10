using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public SettingsData settingsData;
    private Button lastSelected;
    private Canvas InputPanelMenu;
    private TextMeshProUGUI InputPanelTitle;
    private bool panelOpened = false;
    private InputAction rebindingAction;
    public Slider _generalSlider,
        _cartSlider,
        _monstersSlider,
        _weaponsSlider;

    void Start()
    {
        var buttons = ComponentUtils.GetComponentByName<RectTransform>("Input Settings").GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            updateInputButton(button);
        }

        _generalSlider.SetValueWithoutNotify(settingsData.masterVolume);
        _cartSlider.SetValueWithoutNotify(settingsData.cartVolume);
        _monstersSlider.SetValueWithoutNotify(settingsData.monstersVolume);
        _weaponsSlider.SetValueWithoutNotify(settingsData.weaponsVolume);

    }

    public void updateInputButton(Button button)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = button.gameObject.name switch
        {
            "Shoot" => SpriteText.SpritedText("{0}", Global.inputActions.gameplay.Shoot.bindings[0]),
            "Reload" => SpriteText.SpritedText("{0}", Global.inputActions.gameplay.Reload.bindings[0]),
            "Remove Obstacle" => SpriteText.SpritedText("{0}", Global.inputActions.gameplay.RemoveObstacle.bindings[0]),
            "Interact" => SpriteText.SpritedText("{0}", Global.inputActions.gameplay.Interact.bindings[0]),
            "Close Menu" => SpriteText.SpritedText("{0}", Global.inputActions.gameplay.CloseMenu.bindings[0]),
            _ => throw new System.Exception("Attempt to bind unknown action.")
        };

    }

    public void updateInputKeys()
    {
        updateInputButton(lastSelected);
        InputPanelMenu.gameObject.SetActive(false);
    }

    public void changeInputPanel(Button clickedButton)
    {
        InputPanelMenu = ComponentUtils.GetComponentByName<Canvas>("InputPanelMenu");
        InputPanelTitle = ComponentUtils.GetComponentByName<TextMeshProUGUI>("InputPanelTitle");
        lastSelected = clickedButton;
        InputPanelTitle.text = clickedButton.gameObject.name + " Input";
        rebindingAction = clickedButton.gameObject.name switch
        {
            "Shoot" => Global.inputActions.gameplay.Shoot,
            "Reload" => Global.inputActions.gameplay.Reload,
            "Remove Obstacle" => Global.inputActions.gameplay.RemoveObstacle,
            "Interact" => Global.inputActions.gameplay.Interact,
            "Close Menu" => Global.inputActions.gameplay.CloseMenu,
            _ => throw new System.Exception("Attempt to bind unknown action.")
        };
        panelOpened = true;
        rebindingAction.PerformInteractiveRebinding()
            .WithControlsExcluding("/position")
            .WithControlsExcluding("/delta")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(rebind =>
            {
                panelOpened = false;
                updateInputKeys();
                Debug.Log("rebound: " + rebind.selectedControl);
                rebind.Dispose();
                SaveInputBindings();
            })
            .Start();
    }

    private void SaveInputBindings()
    {
        Global.settingsData.inputBindingsJson = Global.inputActions.SaveBindingOverridesAsJson();
    }

    public void GeneralVolume()
    {
        settingsData.masterVolume = Mathf.Clamp01(_generalSlider.value);
        _cartSlider.value = settingsData.masterVolume;
        _monstersSlider.value = settingsData.masterVolume;
        _weaponsSlider.value = settingsData.masterVolume;
    }

    public void CartVolume()
    {
        settingsData.cartVolume = Mathf.Clamp01(_cartSlider.value);
    }

    public void MonstersVolume()
    {
        settingsData.monstersVolume = Mathf.Clamp01(_monstersSlider.value);
    }

    public void WeaponsVolume()
    {
        settingsData.weaponsVolume = Mathf.Clamp01(_weaponsSlider.value);
    }
}

public class ComponentUtils : MonoBehaviour
{
    public static T GetComponentByName<T>(string gameObjectName) where T : Component
    {
        GameObject foundObject = GameObject.Find(gameObjectName);

        if (foundObject != null)
        {
            // Access the component by type after finding the GameObject
            T component = foundObject.GetComponent<T>();

            if (component != null)
            {
                return component;
            }
            else
            {
                Debug.LogWarning(
                    "Component of type " + typeof(T) + " not found on " + gameObjectName
                );
            }
        }
        else
        {
            Debug.LogWarning("GameObject '" + gameObjectName + "' not found");
        }

        return null;
    }
}