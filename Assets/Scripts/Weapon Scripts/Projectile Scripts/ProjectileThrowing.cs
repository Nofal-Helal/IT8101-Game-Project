using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileThrowing : MonoBehaviour
{
    public float throwStrength = 1f;
    private Projectile projectileController;
    private Rigidbody rigidBody;
    public GameObject objectToThrow;
    // Start is called before the first frame update
    void Start()
    {
        projectileController = GetComponent<Projectile>();
        projectileController.OnThrow += ThrowGrenade;
    }
    void OnDestroy()
    {
        projectileController.OnThrow -= ThrowGrenade;
    }
    void ThrowGrenade()
    {
        GameObject thrownObject = Instantiate(objectToThrow, transform.position, transform.rotation);
        thrownObject.GetComponent<Rigidbody>().AddForce(transform.forward * throwStrength);
    }
}
