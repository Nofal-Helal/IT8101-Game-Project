using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shopkeeper : MonoBehaviour
{
    private Camera camera;
    public float distanceToInteract = 10f;
    [HideInInspector]
    public bool isInteractable;
    // Start is called before the first frame update
    void Start()
    {
        isInteractable = false;
        camera = FindObjectOfType<Camera>();
    }
    // Update is called once per frame
    void Update()
    {
        isInteractable = Vector3.Distance(transform.position, camera.transform.position) <= distanceToInteract;
    }
}
