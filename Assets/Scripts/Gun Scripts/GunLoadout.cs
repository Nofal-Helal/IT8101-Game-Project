using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class GunLoadout : MonoBehaviour
{
    private List<GameObject> gunList;
    private int weaponIndex;
    // Start is called before the first frame update
    void Start()
    {
        gunList = GetChildren(gameObject);
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
        setWeapon(gunList, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (weaponIndex == -1)
        {
            return;
        }
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput > 0)
        {
            Debug.Log(scrollInput);
            if (weaponIndex >= gunList.Count)
            {
                return;
            }
            weaponIndex++;
            setWeapon(gunList, weaponIndex);
        }
        if (scrollInput < 0)
        {
            if (weaponIndex <= 0)
            {
                return;
            }
            weaponIndex--;
            Debug.Log("hi -");
            Debug.Log(scrollInput);
            setWeapon(gunList, weaponIndex);
        }
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
    public void setWeapon(List<GameObject> gunList, int weaponIndex)
    {
        if (weaponIndex >= gunList.Count || weaponIndex < 0)
        {
            Debug.Log("NOPE");
            return;
        }
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (i != weaponIndex)
            {
                gunList[i].SetActive(false);
            }
        }
        gunList[weaponIndex].SetActive(true);
    }
}
