using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;


public class FirstPersonCamera : MonoBehaviour
{
    public float cameraSensitivity = 0.25f;
    public GameObject cartObject;
    [Range(0f, 180f)]
    public float rotationRange = 120f;
    private float rotX, rotY;
    public bool acceptingInput = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock mouse cursor
        cartObject = transform.parent.GetChild(0).gameObject;
        Assert.AreEqual("CartObject", cartObject.name);
    }

    void Update()
    {
        if (acceptingInput)
        {
            // Aim Camera
            var mouseDelta = Mouse.current.delta.ReadValue();
            rotY += mouseDelta.x * cameraSensitivity;
            rotX += mouseDelta.y * cameraSensitivity;
            rotX = ClampAngle(rotX, -85f, 85);
            rotY = ClampAngle(rotY, cartObject.transform.eulerAngles.y - rotationRange, cartObject.transform.eulerAngles.y + rotationRange);
            this.transform.eulerAngles = new Vector3(-rotX, rotY, 0);
        }
    }

    public void EnableInput()
    {
        acceptingInput = true;
    }

    public void DisableInput()
    {
        acceptingInput = false;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        float start = (min + max) * 0.5f - 180;
        float floor = Mathf.FloorToInt((angle - start) / 360) * 360;
        return Mathf.Clamp(angle, min + floor, max + floor);
    }
}
