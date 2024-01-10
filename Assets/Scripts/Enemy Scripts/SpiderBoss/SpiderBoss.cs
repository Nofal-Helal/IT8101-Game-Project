using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpiderBoss : MonoBehaviour, IDamageTaker
{
    public float maxHealth = 300;
    public float health = 300;
    private FirstPersonCamera playerCamera;
    private Vector3 initialPosition;
    private AudioSource audioSource;
    private Animation spiderAnimation;
    private SpiderState state;
    public GameObject minion;
    private bool isActive = false;
    private bool isSwitching = false;
    private bool isSpawning = false;
    private float threshold = 40f;
    private float wizardSpeed = 2f;
    private float coolDown = 5f;


    // Start is called before the first frame update
    void Start()
    {
        playerCamera = FindObjectOfType<FirstPersonCamera>();
        audioSource = GetComponent<AudioSource>();
        state = SpiderState.Idle;
        initialPosition = transform.position;
        spiderAnimation = GetComponent<Animation>();
        spiderAnimation.Play("Idle");
    }
    // Update is called once per frame
    void Update()
    {
        if (state == SpiderState.Idle)
        {
            float distance = Vector3.Distance(playerCamera.gameObject.transform.position, transform.position);
            if (distance <= threshold)
            {
                state = SpiderState.Switching;
            }
        }

        if (state == SpiderState.Switching)
        {
            if (!isSwitching)
            {
                StartCoroutine(SwitchPosition());
            }
        }

        if (state == SpiderState.Spawning)
        {
            if (!isSpawning)
            {
                coolDown -= Time.deltaTime;
                if (coolDown <= 0 && !isSpawning)
                {
                    Debug.Log(coolDown);
                    coolDown = 2f;
                    StartCoroutine(SpawnMinion());
                }
            }
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }
    public IEnumerator SwitchPosition()
    {
        isSwitching = true;
        float duration = 0.5f;
        Vector3 previousPosition = transform.position;
        Vector3 targetPosition = initialPosition + new Vector3(Random.Range(-5, 10), 0, Random.Range(0, 5)); // Adjust the values as needed
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            transform.position = Vector3.Lerp(previousPosition, targetPosition, t);
            yield return null;
        }
        transform.position = targetPosition;
        isSwitching = false;
        state = SpiderState.Spawning;
        spiderAnimation.Play("Idle");
    }

    public IEnumerator Die()
    {
        spiderAnimation.Play("Death");
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
    public IEnumerator SpawnMinion()
    {
        isSpawning = true;
        spiderAnimation.Play("Attack");
        Instantiate(minion, gameObject.transform.position + new Vector3(0, 0, 4), gameObject.transform.rotation);
        yield return new WaitForSeconds(0.5f);
        isSpawning = false;
        state = SpiderState.Switching;
        spiderAnimation.Play("Idle");
    }
    public enum SpiderState
    {
        Idle,
        GoBack,
        Switching,
        Spawning
    }
}