using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileThrown : MonoBehaviour
{
    private float timeExisted;
    public float timeTillExplosion = 5f;
    // Start is called before the first frame update
    void Start()
    {
        timeExisted = 0f;
    }
    // Update is called once per frame
    void Update()
    {
        timeExisted += Time.deltaTime;

        if (timeExisted >= timeTillExplosion)
        {
            Explode();
        }
    }
    void Explode()
    {
        Destroy(gameObject);
    }
}
