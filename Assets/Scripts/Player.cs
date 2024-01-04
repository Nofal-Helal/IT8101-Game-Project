using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class Player : MonoBehaviour
{
    public float health = 20f;
    public float maxHealth = 20f;
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
        cartObject = transform.GetChild(0).GetComponent<RailFollower>();
        nextWave ??= cartObject.NextWave;
        circularBar = FindObjectOfType<CircularBar>();
        playableDirector = GameObject.Find("MolemanCutscene").GetComponent<PlayableDirector>();
        GetComponent<PlayerInput>().actions = Global.inputActions.asset;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) { playableDirector.Play(); }
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
}
