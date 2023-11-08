using UnityEngine;

public class Player : MonoBehaviour
{
    public float cameraSensitivity = 4f;
    private GameObject cameraObject;
    private float rotX, rotY;

    // Lock the mouse cursor when the game is focused
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
    }

    void Start()
    {
        cameraObject = GetComponentInChildren<Camera>().gameObject;
    }

    void Update()
    {
        // Aim Camera
        rotY += Input.GetAxis("Mouse X") * cameraSensitivity;
        rotX += Input.GetAxis("Mouse Y") * cameraSensitivity;
        rotX = Mathf.Clamp(rotX, -90f, 90f);
        cameraObject.transform.eulerAngles = new Vector3(-rotX, rotY, 0);
    }
}
