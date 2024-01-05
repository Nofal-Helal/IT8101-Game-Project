using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private BoxCollider boxCollider;
    private GameObject shopMenu;
    private Shopkeeper shopkeeper;
    GameInputActions.GameplayActions inputActions;

    void Start()
    {
        inputActions = Global.inputActions.gameplay;
        shopMenu = GetComponentInChildren<Canvas>().gameObject;
        shopkeeper = GetComponentInChildren<Shopkeeper>();
    }

    void Update()
    {
        if (!shopkeeper.isInteractable)
        {
            return;
        }

        if (inputActions.Interact.IsPressed())
        {
            shopMenu.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
        if (inputActions.CloseMenu.IsPressed())
        {
            shopMenu.SetActive(false);
            Cursor.visible = false;
            Time.timeScale = 1;
        }
    }
}
