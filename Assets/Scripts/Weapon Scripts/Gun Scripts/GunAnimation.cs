using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GunAnimation : MonoBehaviour
{
    private Animator animator;
    private Gun gunController;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        gunController = GetComponent<Gun>();
        gunController.OnShoot += PlayShootAnimation;
        gunController.OnReload += PlayReloadAnimation;
    }

    void OnDestroy()
    {
        gunController.OnShoot -= PlayShootAnimation;
        gunController.OnReload -= PlayReloadAnimation;
    }

    void PlayShootAnimation()
    {
        animator.Play("Fire");
    }

    void PlayReloadAnimation()
    {
        animator.Play("Reload");
    }
}