using System.Dynamic;
using Palmmedia.ReportGenerator.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    // TODO: store and load settings (persistence)
    // TODO: update unity inputsystem input asset

    public SettingsData settingsData;
    private Button lastSelected;
    private Button shootInputButton;
    private Button reloadInputButton;
    private Canvas InputPanelMenu;
    private TextMeshProUGUI InputPanelTitle;
    private bool panelOpened = false;
    public Slider _generalSlider,
        _cartSlider,
        _monstersSlider,
        _weaponsSlider;

    void Start()
    {
        shootInputButton = ComponentUtils.GetComponentByName<Button>("Shoot");
        reloadInputButton = ComponentUtils.GetComponentByName<Button>("Reload");

        updateInputButton(shootInputButton);
        updateInputButton(reloadInputButton);
    }

    public void updateInputButton(Button button)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (button.gameObject.name == "Shoot")
        {
            buttonText.text = "Shoot: " + Global.shootInput;
            shootInputButton = button;
        }
        else if (button.gameObject.name == "Reload")
        {
            buttonText.text = "Reload: " + Global.reloadInput;
            reloadInputButton = button;
        }
    }

    public void updateInputKeys(KeyCode selectedInput)
    {
        if (lastSelected.gameObject.name == "Shoot")
        {
            Global.shootInput = selectedInput;
        }
        else if (lastSelected.gameObject.name == "Reload")
        {
            Global.reloadInput = selectedInput;
        }
        updateInputButton(lastSelected);
        InputPanelMenu.gameObject.SetActive(false);
    }

    public void changeInputPanel(Button clickedButton)
    {
        InputPanelMenu = ComponentUtils.GetComponentByName<Canvas>("InputPanelMenu");
        InputPanelTitle = ComponentUtils.GetComponentByName<TextMeshProUGUI>("InputPanelTitle");
        lastSelected = clickedButton;
        InputPanelTitle.text = clickedButton.gameObject.name + " Input";
        panelOpened = true;
    }

    void Update()
    {
        // Check if the defined input key is pressed
        if (Input.anyKeyDown) // "MyKey" should match the name in the Input Manager
        {
            if (panelOpened)
            {
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        updateInputKeys(keyCode);
                        panelOpened = false;
                        break;
                    }
                }
            }
        }
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