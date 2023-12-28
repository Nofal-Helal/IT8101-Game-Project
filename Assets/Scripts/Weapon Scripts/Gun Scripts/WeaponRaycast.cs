using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponRaycast : MonoBehaviour
{
    private Camera camera;
    private LayerMask layerMask;
    public float distance = 1000f;
    // Start is called before the first frame update
    void Start()
    {
        // This makes it ignore the cart collider
        // the collidor is right infront of the camera, so it hits it when it shouldn't.
        layerMask = ~(1 << 8);
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);
        Debug.Log("yuh");
        if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
        {
            Debug.Log("yuhhhh");
            Debug.Log(hitInfo.collider.GetComponent<Transform>().name);
        };
    }
}
