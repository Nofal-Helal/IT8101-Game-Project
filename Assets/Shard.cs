using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.CodeAnalysis;
using UnityEngine;

public class Shard : MonoBehaviour
{
    private FirstPersonCamera firstPersonCamera;
    private Player player;
    public float damage = 15f;
    public float threshold = 5f;
    private float speed = 100f;
    public GameObject explosionEffect;
    public AudioClip explosionClip;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        player = FindAnyObjectByType<Player>();
        firstPersonCamera = FindAnyObjectByType<FirstPersonCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(firstPersonCamera.transform.position, transform.position);
        Debug.Log("ShardDistance: " + distance);
        Vector3 direction = firstPersonCamera.transform.position - transform.position;

        // Normalize the direction to get a unit vector
        Vector3 normalizedDirection = direction.normalized;

        // Calculate the new position based on the direction and follow speed
        Vector3 newPosition = transform.position + normalizedDirection * Time.deltaTime;

        // Update the follower's position
        transform.position = newPosition;
        if (distance <= threshold)
        {
            Destroy(gameObject);
        };
    }
    public IEnumerator Blowup()
    {
        audioSource.clip = explosionClip;
        audioSource.Play();
        Instantiate(explosionEffect, gameObject.transform);
        if (player != null && player.isAlive())
        {
            ((IDamageTaker)player).TakeDamage(damage);
        }
        Destroy(gameObject);
        yield return null;
    }
}
