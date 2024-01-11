using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour, IDamageTaker
{
    public PlayerData playerData;
    public float health = 20f;
    public bool isAlive = true;
    private bool inWave;
    private RailFollower cartObject;
    public Wave nextWave;
    public float secondsToRemoveObstacle = 2f;
    private bool removingObstacle;
    private float obstacleProgress = 0f;
    private CircularBar circularBar;
    private AudioSource audioSource;
    public AudioClip hurtClip;
    public event Action OnDeath;

    private void Start()
    {
        Assert.AreEqual("CartObject", transform.GetChild(0).name);
        health = playerData.maxHealth;
        cartObject = GetComponentInChildren<RailFollower>();
        nextWave ??= cartObject.NextWave;
        circularBar = FindObjectOfType<CircularBar>();
        audioSource = GameObject.Find("player_audio").GetComponent<AudioSource>();
        GetComponent<PlayerInput>().actions = Global.inputActions.asset;
    }

    private void Update()
    {
        // if (!inWave && nextWave)
        // {
        //     if (cartObject.distance >= nextWave.Distance)
        //     {
        //         inWave = true;
        //         nextWave.SpawnNextSubWave();
        //         _ = cartObject.railTrack.Spline.TryGetObjectData("waves", out var waves);
        //         _ = waves.RemoveDataPoint(nextWave.Distance);
        //         nextWave = cartObject.NextWave;
        //     }
        // }

        if (removingObstacle)
        {
            obstacleProgress += Time.deltaTime;
            circularBar.progress = obstacleProgress / secondsToRemoveObstacle;

            if (obstacleProgress >= secondsToRemoveObstacle)
            {
                cartObject.OnRemoveObstacle();
                inWave = false; // TODO: Maybe set it to false when the wave actually ends
                obstacleProgress = 0f;
                removingObstacle = false;
                circularBar.visible = false;
            }
        }
    }

    // hold o to remove obstacle
    public void OnRemoveObstacle(InputAction.CallbackContext ctx)
    {
        // button is pressed while cart is stopped
        if (
            ctx.performed
            && cartObject.speed == 0
            && cartObject.nextObstacle != null
            && cartObject.nextObstacle.IsVisible
        )
        {
            removingObstacle = true;
            circularBar.visible = true;
            circularBar.progress = obstacleProgress;
            circularBar.target = cartObject.nextObstacle.transform;
        }

        // button is released
        if (ctx.canceled)
        {
            removingObstacle = false;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        audioSource.pitch = 1f + Random.Range(-0.3f, 0.3f);
        audioSource.PlayOneShot(hurtClip);
        health -= damageAmount;
        if (health <= 0)
        {
            Die();
        }
    }

    // This function is used to set the new value of the maxHealth after
    // the health increase upgrade is bought.
    public void IncreaseMaxHealth()
    {
        playerData.maxHealth = playerData.maxHealth * (1f + (1f / 3f) * playerData.healthIncreaseLevel);
    }
    public void Die()
    {
        if (isAlive)
        {
            Debug.Log("You've died. Unlucky.");
            SceneTransition.Fade("Scenes/GameOver");
        }
        isAlive = false;
    }

    public bool IsAlive()
    {
        return health > 0f;
    }
}
