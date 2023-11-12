using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSway : MonoBehaviour
{
    public float moveAmount = 0.005f; // Amount of sway movement
    public float moveSpeed = 2f; // Speed of the sway movement

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        float moveX = Mathf.Sin(Time.time * moveSpeed) * moveAmount;
        float moveY = Mathf.Sin(Time.time * moveSpeed * 1.5f) * moveAmount;

        // Apply the sway movement to the gun's local position
        Vector3 newPosition = initialPosition + new Vector3(moveX, moveY, 0f);
        transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, Time.deltaTime * 5f);
    }
}
