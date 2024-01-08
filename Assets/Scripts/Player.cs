using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class Player : MonoBehaviour
{
    public float health = 20f;
    public int score = 0;
    public int gold = 0;
    public int healthIncreaseLevel = 0;
    public int damageBoostLevel = 0;
    public float maxHealth = 100f;
    private bool inWave;
    private RailFollower cartObject;
    public Wave nextWave;
    public float secondsToRemoveObstacle = 2f;
    private bool removingObstacle;
    private float obstacleProgress = 0f;
    private CircularBar circularBar;
    private PlayableDirector playableDirector;

    private void Start()
    {
        Assert.AreEqual("CartObject", transform.GetChild(0).name);
        // The 1/3 value here is arbitrary, but I chose it so it when the player reaches the highest level it doubles his health
        maxHealth = maxHealth * (1f + (1f / 3f) * healthIncreaseLevel);
        cartObject = GetComponentInChildren<RailFollower>();
        nextWave ??= cartObject.NextWave;
        circularBar = FindObjectOfType<CircularBar>();
        // playableDirector = GameObject.Find("MolemanCutscene").GetComponent<PlayableDirector>();
        GetComponent<PlayerInput>().actions = Global.inputActions.asset;
    }

    private void Update()
    {
        Debug.Log(nextWave);
        // if (Input.GetKeyDown(KeyCode.A)) { playableDirector.Play(); }
        if (!inWave && nextWave)
        {
            if (cartObject.distance >= nextWave.Distance)
            {
                inWave = true;
                nextWave.SpawnNextSubWave();
                _ = cartObject.railTrack.Spline.TryGetObjectData("waves", out var waves);
                _ = waves.RemoveDataPoint(nextWave.Distance);
                nextWave = cartObject.NextWave;
            }
        }

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
        maxHealth = maxHealth * (1f + (1f / 3f) * healthIncreaseLevel);
    }
    public void Die()
    {
        Debug.Log("You've died. Unlucky.");
        Destroy(gameObject);
    }

    public bool isAlive()
    {
        return health > 0f;
    }
}
