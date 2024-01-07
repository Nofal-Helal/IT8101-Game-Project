using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YourGameNamespace;

public class ProjectileThrown : MonoBehaviour
{
    private float timeExisted;
    public float timeTillExplosion = 2f;
    public GunData projectileData;
    private CanvasRenderer canvasRenderer;
    private SphereCollider impactSphere;
    public List<GameObject> detectedEnemies = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        timeExisted = 0f;
        impactSphere = GetComponentInChildren<SphereCollider>();

        if (projectileData == null)
        {
            Debug.Log("Projectile data isn't attached");
            return;
        }
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
    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            detectedEnemies.Add(collider.gameObject);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            detectedEnemies.Remove(collider.gameObject);
        }
    }
    void Explode()
    {
        Destroy(gameObject);
        foreach (GameObject enemy in detectedEnemies)
        {
            Enemy enemyValue = enemy.GetComponent<Enemy>();
            enemyValue.health -= projectileData.shortRangeDamage;
        }
    }

}
