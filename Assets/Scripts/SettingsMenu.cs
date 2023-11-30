using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public static KeyCode shootInput = KeyCode.Mouse0;
    public static KeyCode reloadInput = KeyCode.R;

    private Button lastSelected;
    private Button shootInputButton;
    private Button reloadInputButton;
    private Canvas InputPanelMenu;
    private TextMeshProUGUI InputPanelTitle;
    private bool panelOpened = false;

    void Start(){
        shootInputButton = GetComponentByName<Button>("Shoot");
        reloadInputButton = GetComponentByName<Button>("Reload");

        updateInputButton(shootInputButton);
        updateInputButton(reloadInputButton);
    }

    T GetComponentByName<T>(string gameObjectName) where T : Component
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
                Debug.LogWarning("Component of type " + typeof(T) + " not found on " + gameObjectName);
            }
        }
        else
        {
            Debug.LogWarning("GameObject '" + gameObjectName + "' not found");
        }

        return null;
    }

    public void updateInputButton(Button button){
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if(button.gameObject.name == "Shoot"){
            buttonText.text = "Shoot: "+shootInput;
            shootInputButton = button;
        }else if(button.gameObject.name == "Reload"){
            buttonText.text = "Reload: "+reloadInput;
            reloadInputButton = button;
        }
    }

    public void updateInputKeys(KeyCode selectedInput){
        if(lastSelected.gameObject.name == "Shoot"){
            shootInput = selectedInput;
        }else if(lastSelected.gameObject.name == "Reload"){
            reloadInput = selectedInput;
        }
        updateInputButton(lastSelected);
        InputPanelMenu.gameObject.SetActive(false);
    }

    public void changeInputPanel(Button clickedButton){
        InputPanelMenu = GetComponentByName<Canvas>("InputPanelMenu");
        InputPanelTitle = GetComponentByName<TextMeshProUGUI>("InputPanelTitle");
        lastSelected = clickedButton;
        InputPanelTitle.text = clickedButton.gameObject.name + " Input";
        panelOpened = true;
    }

    void Update()
    {
        // Check if the defined input key is pressed
        if (Input.anyKeyDown) // "MyKey" should match the name in the Input Manager
        {
            if(panelOpened){
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
}
