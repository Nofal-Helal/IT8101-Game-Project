using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private GameObject shopMenu;
    private Shopkeeper shopkeeper;
    private FirstPersonCamera firstPersonCamera;
    public readonly int healthBoostCost = 10;
    public readonly int damageBoostCost = 10;
    GameInputActions.GameplayActions inputActions;

    void Start()
    {
        inputActions = Global.inputActions.gameplay;
        shopMenu = gameObject.transform.GetChild(0).gameObject;
        shopkeeper = gameObject.transform.GetChild(1).gameObject.GetComponent<Shopkeeper>();
        firstPersonCamera = FindObjectOfType<FirstPersonCamera>();
    }

    void Update()
    {
        if (!shopkeeper.isInteractable)
        {
            return;
        }

        if (inputActions.Interact.IsPressed())
        {
            firstPersonCamera.acceptingInput = false;
            Time.timeScale = 0;
            shopMenu.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if (inputActions.CloseMenu.IsPressed())
        {
            firstPersonCamera.acceptingInput = true;
            shopMenu.SetActive(false);
            Cursor.visible = false;
            Time.timeScale = 1;
        }
    }
}