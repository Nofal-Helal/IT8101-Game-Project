using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class WeaponRaycast : MonoBehaviour
{
    private new Camera camera;
    private LayerMask layerMask;
    private Gun gunController;
    public float distance = 1000f;
    // Start is called before the first frame update
    void Start()
    {
        // This makes it ignore the cart collider
        // the collidor is right infront of the camera, so it hits it when it shouldn't.
        layerMask = ~(1 << 8);
        gunController = GetComponent<Gun>();
        gunController.OnShoot += ShootRay;

        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void OnDestroy()
    {
        gunController.OnShoot -= ShootRay;
    }

    void ShootRay()
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
        {
            // Please make sure that the GameObject that has the collider has the tag Enemy
            if (hitInfo.collider.gameObject.transform.tag == "Enemy")
            {
                var enemy = hitInfo.collider.gameObject.GetComponent<Enemy>();
                var distanceToEnemy = Vector3.Distance(ray.origin, hitInfo.point);
                enemy.TakeDamage(gunController.GetDamageValue(distanceToEnemy));
            }
        };
    }
}

