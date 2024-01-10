using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.CodeAnalysis;
using UnityEngine;

public class Shard : MonoBehaviour, IDamageTaker
{
    private FirstPersonCamera firstPersonCamera;
    private Player player;
    private float maxHealth = 15;
    private float health = 15;
    public float damage = 15f;
    public float threshold = 5f;
    private float speed = 10f;
    public GameObject explosionEffect;
    private AudioSource audioSource;
    public AudioClip explosionClip;
    private WizardBoss wizardBoss;
    // Start is called before the first frame update
    void Start()
    {
        player = FindAnyObjectByType<Player>();
        firstPersonCamera = FindAnyObjectByType<FirstPersonCamera>();
        wizardBoss = FindObjectOfType<WizardBoss>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            Debug.Log("Player not found");
        }

        float distance = Vector3.Distance(firstPersonCamera.transform.position, transform.position);
        Debug.Log("ShardDistance: " + distance);
        Vector3 direction = firstPersonCamera.transform.position - transform.position;

        // Normalize the direction to get a unit vector
        Vector3 normalizedDirection = direction.normalized;

        // Calculate the new position based on the direction and follow speed
        Vector3 newPosition = transform.position + normalizedDirection * Time.deltaTime * speed;

        // Update the follower's position
        transform.position = newPosition;
        if (distance <= threshold)
        {
            Blowup();
        };
    }
    public void Blowup()
    {
        audioSource.clip = explosionClip;
        audioSource.Play();
        Instantiate(explosionEffect, gameObject.transform.position, gameObject.transform.rotation);
        if (player != null && player.isAlive)
        {
            Debug.Log("Are we hitting this dude?");
            ((IDamageTaker)player).TakeDamage(damage);
        }
        Destroy(gameObject);
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        audioSource.clip = explosionClip;
        audioSource.Play();

        if (health < 0)
        {
            Break();
        }
    }
    public void Break()
    {
        audioSource.clip = explosionClip;
        audioSource.Play();
        wizardBoss.TakeDamage(maxHealth);
        Destroy(gameObject);
    }
}
