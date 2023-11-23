using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    public float health = 20f;
    public float maxHealth = 20f;


    public float cameraSensitivity = 1f;
    private GameObject cameraObject;
    private GameObject cartObject;
    private float rotX, rotY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock mouse cursor
        cameraObject = GetComponentInChildren<Camera>().gameObject;
        Assert.AreEqual("Player Camera", cameraObject.name);
        cartObject = transform.GetChild(0).gameObject;
        Assert.AreEqual("CartObject", cartObject.name);
    }

    void Update()
    {
        // Vector2 mouse = Mouse.current.delta.value;
        // Aim Camera
        rotY += Input.GetAxis("Mouse X") * cameraSensitivity;
        rotX += Input.GetAxis("Mouse Y") * cameraSensitivity;
        rotX = ClampAngle(rotX, -85f, 85);
        rotY = ClampAngle(rotY, cartObject.transform.eulerAngles.y - 120f, cartObject.transform.eulerAngles.y + 120f);
        cameraObject.transform.eulerAngles = new Vector3(-rotX, rotY, 0);
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        float start = (min + max) * 0.5f - 180;
        float floor = Mathf.FloorToInt((angle - start) / 360) * 360;
        return Mathf.Clamp(angle, min + floor, max + floor);
    }
}
