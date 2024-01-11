using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class WizardBoss : MonoBehaviour
{
  public float maxHealth = 300;
  public float health = 300;
  private WizardState state;
  private bool isActive = false;
  private bool isShooting = false;
  private bool isSwitching = false;
  private float threshold = 10f;
  private float wizardSpeed = 2f;
  private float coolDown = 1f;
  private FirstPersonCamera playerCamera;
  private Animator wizardAnimator;
  private Vector3 initialPosition;
  public GameObject shardObject;
  private AudioSource audioSource;
  public GameObject wizardHealthUI;

  // Start is called before the first frame update
  void Start()
  {
    playerCamera = FindObjectOfType<FirstPersonCamera>();
    wizardAnimator = GetComponent<Animator>();
    audioSource = GetComponent<AudioSource>();
    state = WizardState.Idle;
    initialPosition = transform.position;
    wizardHealthUI.SetActive(false);
  }
  // Update is called once per frame
  void Update()
  {
    if (state == WizardState.Idle)
    {
      float distance = Vector3.Distance(playerCamera.gameObject.transform.position, transform.position);
      Debug.Log(distance);
      if (distance <= threshold)
      {
        wizardHealthUI.SetActive(true);
        state = WizardState.GoBack;
      }
    }

    if (state == WizardState.GoBack)
    {
      StartCoroutine(GoBack());
    }

    if (state == WizardState.Switching)
    {
      if (!isSwitching)
      {
        StartCoroutine(SwitchPosition());
      }
    }

    if (state == WizardState.Attacking)
    {
      if (!isShooting)
      {
        coolDown -= Time.deltaTime;
        if (coolDown <= 0 && !isShooting)
        {
          Debug.Log(coolDown);
          coolDown = 2f;
          StartCoroutine(ShootShard());
        }
      }
    }
  }
  public IEnumerator GoBack()
  {
    float duration = 1.0f;
    Vector3 targetPosition = initialPosition + new Vector3(0, 0, 15); // Adjust the values as needed
    float elapsedTime = 0;
    while (elapsedTime < duration)
    {
      elapsedTime += Time.deltaTime;
      float t = Mathf.Clamp01(elapsedTime / duration);
      transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
      yield return null;
    }
    transform.position = targetPosition;
    state = WizardState.Switching;
  }
  public IEnumerator ShootShard()
  {
    isShooting = true;
    wizardAnimator.Play("attack_short_001");
    audioSource.Play();
    Instantiate(shardObject, gameObject.transform.position + Vector3.up * 7, gameObject.transform.rotation);
    yield return new WaitForSeconds(1f);
    isShooting = false;
    state = WizardState.Switching;
  }

  public void TakeDamage(float damage)
  {
    Debug.Log("I should be taking Damage");
    wizardAnimator.Play("damage_001");
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
    Vector3 targetPosition = initialPosition + new Vector3(0, 0, 15) + new Vector3(Random.Range(-10, 10), Random.Range(0, 10), 0); // Adjust the values as needed
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
    state = WizardState.Attacking;
  }

  public IEnumerator Die()
  {
    wizardAnimator.Play("dead");
    yield return new WaitForSeconds(5f);
    wizardHealthUI.SetActive(false);
    Destroy(gameObject);
  }
}

public enum WizardState
{
  Idle,
  GoBack,
  Switching,
  Attacking,
}