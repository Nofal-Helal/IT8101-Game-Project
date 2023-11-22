using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class GunLoadout : MonoBehaviour
{
    public List<GameObject> gunList;
    public int weaponIndex;
    private Animator animator;
    private bool switching;
    // Start is called before the first frame update
    void Start()
    {
        gunList = GetChildren(gameObject);
        animator = gameObject.GetComponent<Animator>();
        switching = false;

        if (gunList.Count == 0)
        {
            weaponIndex = -1;
        }
        else
        {
            weaponIndex = 0;
        }
        foreach (GameObject gun in gunList)
        {
            gun.SetActive(false);
        }
        gunList[0].SetActive(true);
        StartCoroutine(Pullout());
    }

    // Update is called once per frame
    void Update()
    {

        if (!switching && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            if (weaponIndex == -1)
            {
                return;
            }
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (scrollInput > 0)
            {
                Debug.Log(scrollInput);
                if (weaponIndex >= gunList.Count - 1)
                {
                    return;
                }
                weaponIndex++;
                Debug.Log("hi +");
                Debug.Log(weaponIndex);
                Debug.Log(scrollInput);
                StartCoroutine(Switch(gunList, weaponIndex));
            }
            if (scrollInput < 0)
            {
                Debug.Log(scrollInput);
                if (weaponIndex <= 0)
                {
                    return;
                }
                weaponIndex--;
                Debug.Log("hi -");
                Debug.Log(weaponIndex);
                Debug.Log(scrollInput);
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
    public List<GameObject> GetChildren(GameObject gameObject)
    {
        List<GameObject> childrenList = new List<GameObject>();
        Transform parent = gameObject.transform;
        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject childGameObject = parent.GetChild(i).gameObject;
            Debug.Log(childGameObject.name);
            childrenList.Add(childGameObject);
        }
        return childrenList;
    }
}
