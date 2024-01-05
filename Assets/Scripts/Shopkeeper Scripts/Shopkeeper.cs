using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shopkeeper : MonoBehaviour
{
    private Camera camera;
    public float distanceToInteract = 10f;
    public bool isInteractable;
    GameInputActions.GameplayActions inputActions;
    // Start is called before the first frame update
    void Start()
    {
        isInteractable = false;
        camera = FindObjectOfType<Camera>();
        inputActions = Global.inputActions.gameplay;
    }

    // Update is called once per frame
    void Update()
    {
        isInteractable = Vector3.Distance(transform.position, camera.transform.position) <= distanceToInteract;
        Debug.Log("Ghost position: " + transform.position.ToString());
        Debug.Log("Player position: " + camera.transform.position.ToString());
        Debug.Log(Vector3.Distance(transform.gameObject.transform.position, camera.transform.position));
    }
}
