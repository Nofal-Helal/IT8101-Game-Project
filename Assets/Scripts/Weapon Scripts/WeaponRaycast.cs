using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class CameraRaycast : MonoBehaviour
{
    private new Camera camera;
    private LayerMask layerMask;
    private Gun gunController;
    private Player player;
    public float distance = 1000f;
    public RaycastHit raycastHit;
    public float distanceToRay;
    public string aimedAtTag;
    // Start is called before the first frame update
    void Start()
    {
        // This makes it ignore the cart collider
        // the collidor is right infront of the camera, so it hits it when it shouldn't.
        layerMask = ~(1 << 8);
        player = FindObjectOfType<Player>();
        gunController = GetComponent<Gun>();
        gunController.OnShoot += ShootRay;
        aimedAtTag = "";
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void OnDestroy()
    {
        gunController.OnShoot -= ShootRay;
    }

    void Update()
    {
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        if (Physics.Raycast(ray, out raycastHit, distance, layerMask))
        {
            distanceToRay = Vector3.Distance(ray.origin, raycastHit.point);
            aimedAtTag = raycastHit.collider.gameObject.tag;
            Debug.Log(raycastHit.collider.gameObject.name);
        }
        // if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
        // {
        //     // Please make sure that the GameObject that has the collider has the tag Enemy
        //     if (hitInfo.collider.gameObject.transform.tag == "Enemy")
        //     {
        //         BaseUniversal enemy = hitInfo.collider.gameObject.GetComponent<BaseUniversal>();
        //         var distanceToEnemy = Vector3.Distance(ray.origin, hitInfo.point);
        //         enemy.TakeDamage(gunController.GetDamageValue(distanceToEnemy));
        //     }
        // };
    }

    void ShootRay()
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
        {
            // Please make sure that the GameObject that has the collider has the tag Enemy
            if (hitInfo.collider.CompareTag("Enemy"))
            {
                Debug.Log("The enemy should be getting shot !!!");
                BaseUniversal enemy = hitInfo.collider.gameObject.GetComponent<BaseUniversal>();
                var distanceToEnemy = Vector3.Distance(ray.origin, hitInfo.point);
                // The 1/3 value here is arbitrary, but I chose it so it when the player reaches the highest level it doubles his damage output
                Debug.Log(gunController.GetDamageValue(distanceToEnemy) * (1 + ((1 / 3) * player.damageBoostLevel)));
                enemy.TakeDamage(gunController.GetDamageValue(distanceToEnemy) * (1 + ((1 / 3) * player.damageBoostLevel)));
            }
        };
    }
}

