using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class CameraRaycast : MonoBehaviour
{
    private Camera playerCamera;
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
        playerCamera = GameObject.Find("Player Camera").GetComponent<Camera>();
    }

    void OnDestroy()
    {
        gunController.OnShoot -= ShootRay;
    }

    void Update()
    {
    }

    void ShootRay()
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
        {
            // Please make sure that the GameObject that has the collider has the tag Enemy
            var distanceToEnemy = Vector3.Distance(ray.origin, hitInfo.point);
            // The 1/3 value here is arbitrary, but I chose it so it when the player reaches the highest level it doubles his damage output
            float damageBoostMultiplier = 1f + (1f / 3f * player.damageBoostLevel);

            float damage = gunController.GetDamageValue(distanceToEnemy) * damageBoostMultiplier;

            if (hitInfo.collider.gameObject.TryGetComponent(out IDamageTaker enemy))
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}

