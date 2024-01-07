using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class GunLoadout : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> gunList;
    [HideInInspector]
    public int weaponIndex;
    [HideInInspector]
    public GameObject currentGun;
    private Animator animator;
    private bool isActive;
    private bool switching;
    // Start is called before the first frame update
    void Start()
    {
        gunList = GetChildren(gameObject);
        if (gunList.Count == 0)
        {
            return;
        }
        foreach (GameObject gun in gunList)
        {
            gun.SetActive(false);
        }
        weaponIndex = 0;
        currentGun = gunList[weaponIndex];
        currentGun.SetActive(true);
        animator = gameObject.GetComponent<Animator>();
        switching = false;
        isActive = true;
        StartCoroutine(Pullout());
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGun.GetComponent<Gun>() != null)
        {
            isActive = currentGun.GetComponent<Gun>().gunData.reloading || currentGun.GetComponent<Gun>().gunData.shooting;
        }
        else if (currentGun.GetComponent<Projectile>() != null)
        {
            isActive = currentGun.GetComponent<Projectile>().gunData.reloading || currentGun.GetComponent<Projectile>().gunData.shooting;
        }

        if (!switching && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !(isActive))
        {
            if (weaponIndex == -1)
            {
                return;
            }
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (scrollInput > 0)
            {
                if (weaponIndex >= gunList.Count - 1)
                {
                    return;
                }
                weaponIndex++;
                currentGun = gunList[weaponIndex];
                StartCoroutine(Switch(gunList, weaponIndex));
            }
            if (scrollInput < 0)
            {
                if (weaponIndex <= 0)
                {
                    return;
                }
                weaponIndex--;
                currentGun = gunList[weaponIndex];
                StartCoroutine(Switch(gunList, weaponIndex));
            }
        }
    }
    public IEnumerator Pullout()
    {
        switching = true;
        animator.Play("Pullout");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        switching = false;
    }
    public IEnumerator Switch(List<GameObject> gunList, int weaponIndex)
    {
        switching = true;
        animator.Play("Switching");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length / 2);
        SetWeapon(gunList, weaponIndex);
        animator.Play("Pullout");
        switching = false;
    }

    public void SetWeapon(List<GameObject> gunList, int weaponIndex)
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (i != weaponIndex)
            {
                gunList[i].SetActive(false);
            }
        }
        gunList[weaponIndex].SetActive(true);
    }

    // It takes in a weapon name as a string
    // because the value is coming from Shop UI button
    public void AddToLoadout(GameObject weapon)
    {
        if (gunList.Find(weapon => weapon.transform.name == weapon.transform.name))
        {
            Debug.Log("You already have this weapon");
            return;
        }
        GameObject newWeapon = Instantiate(weapon, gameObject.transform);
        newWeapon.SetActive(false);
        // Resets the list
        gunList = GetChildren(gameObject);
    }

    public List<GameObject> GetChildren(GameObject gameObject)
    {
        List<GameObject> childrenList = new List<GameObject>();
        Transform parent = gameObject.transform;
        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject childGameObject = parent.GetChild(i).gameObject;
            childrenList.Add(childGameObject);
        }
        return childrenList;
    }
}