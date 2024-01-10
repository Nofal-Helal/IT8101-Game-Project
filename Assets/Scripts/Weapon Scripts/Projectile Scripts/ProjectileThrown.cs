using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileThrown : MonoBehaviour
{
    private Player player;
    private float timeExisted;
    public float timeTillExplosion = 2f;
    public GameObject explosionEffect;
    public GunData projectileData;
    private bool blowingUp;
    public AudioClip explosionClip;
    private AudioSource audioSource;
    public List<GameObject> detectedEnemies = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        timeExisted = 0f;
        blowingUp = false;
        player = FindObjectOfType<Player>();
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        timeExisted += Time.deltaTime;
        if (timeExisted >= timeTillExplosion && !blowingUp)
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
    public void Explode()
    {
        blowingUp = true;
        audioSource.clip = explosionClip;
        if (explosionClip.loadState == AudioDataLoadState.Loaded)
        {
            audioSource.Play();
        }
        Instantiate(explosionEffect, gameObject.transform.position, gameObject.transform.rotation);
        if (detectedEnemies != null)
        {
            foreach (GameObject enemy in detectedEnemies)
            {
                var distanceToEnemy = Vector3.Distance(gameObject.transform.position, enemy.transform.position);
                float damageBoostMultiplier = 1f + (1f / 3f * player.damageBoostLevel);

                if (enemy.gameObject.TryGetComponent(out IDamageTaker enemyData))
                {
                    enemyData.TakeDamage(Projectile.GetDamageValue(distanceToEnemy, projectileData) * damageBoostMultiplier);
                }
            }
        }
        Destroy(gameObject);
    }
}
