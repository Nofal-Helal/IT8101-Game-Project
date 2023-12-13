using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunSpray : MonoBehaviour
{
    public float sprayThreshold;
    private float sprayTime;
    private 
    // Start is called before the first frame update
    void Start()
    {
        sprayTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            sprayTime += Time.deltaTime;
            return; 
        } 
        sprayTime = 0f;
    }
}
