using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Level1Script : MonoBehaviour
{
    private Camera playerCamera;
    private FirstPersonCamera firstPersonCamera;
    private Camera cutsceneCamera;
    private GameObject gunView;
    private Moleman moleman;
    private Player player;
    private RailFollower cartObject;
    private Obstacle molemanObstacle;
    private PlayableDirector cutscene;
    private bool cutsceneStarted = false;

    void Start()
    {
        playerCamera = GameObject.Find("Player Camera").GetComponent<Camera>();
        firstPersonCamera = FindFirstObjectByType<FirstPersonCamera>();
        cutsceneCamera = GameObject.Find("CutsceneCamera").GetComponent<Camera>();
        gunView = GameObject.Find("Gun View");
        moleman = FindObjectOfType<Moleman>();
        player = FindObjectOfType<Player>();
        cartObject = player.transform.GetChild(0).GetComponent<RailFollower>();
        molemanObstacle = GameObject.Find("Obstacle_Moleman").GetComponent<Obstacle>();
        cutscene = GameObject.Find("MolemanCutscene").GetComponent<PlayableDirector>();

        cutsceneCamera.gameObject.SetActive(false);
        cartObject.ReachedEndOfTrack += TransitionToNextLevel;
    }

    void Update()
    {
        if (!cutsceneStarted)
            if (molemanObstacle.Distance - cartObject.distance <= Mathf.Epsilon)
            {
                cutsceneStarted = true;
                cutscene.Play();
            }
    }

    public void CutsceneStart()
    {
        firstPersonCamera.DisableInput();
        Global.inputActions.Disable();
        StartCoroutine(TweenCamera1());
    }

    public void CutscenePart2()
    {
        StartCoroutine(TweenCamera2());
        if (molemanObstacle)
        {
            cartObject.OnRemoveObstacle();
        }
    }

    public void CutsceneEnd()
    {
        gunView.SetActive(true);
        cutsceneCamera.gameObject.SetActive(false);
        firstPersonCamera.EnableInput();
        Global.inputActions.Enable();
        // Global.inputActions.gameplay.RemoveObstacle.Disable(); // disable removing the obstacle in the boss fight
        moleman.ChangeState(Moleman.State.RangedAttack);
    }


    private IEnumerator TweenCamera1()
    {
        yield return firstPersonCamera.transform.TweenRotation(moleman.transform.position, 0.5f, 2.25f);
        // switch to cutscene camera
        gunView.SetActive(false);
        // cutsceneCamera.transform.parent.SetPositionAndRotation(playerCamera.transform.position, playerCamera.transform.rotation);
        cutsceneCamera.gameObject.SetActive(true);
    }

    private IEnumerator TweenCamera2()
    {
        Quaternion target = new Quaternion(0.00774417073f, -0.0256361142f, 0.000296277722f, 0.999641299f);
        yield return firstPersonCamera.transform.TweenRotation(target, 0.5f);
    }

    private void TransitionToNextLevel()
    {
        Global.playerData.lastCompletedLevel = 0;
        TransitionSceneScript.nextScene = "Scenes/Level_2/Level 2";
        SceneTransition.Fade("Scenes/Transition1to2");
    }

}


static public class TransformExtensions
{
    public static IEnumerator TweenRotation(this Transform transform, Quaternion targetRotation, float duration)
    {
        Quaternion initial = transform.rotation;
        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(initial, targetRotation, time / duration);
            yield return 0;
        }
    }

    public static IEnumerator TweenRotation(this Transform transform, Vector3 targetPosition, float duration, float lookHeightY = 0)
    {
        Vector3 lookHeight = new(0, lookHeightY, 0);
        Quaternion target = Quaternion.LookRotation(targetPosition + lookHeight - transform.position);
        yield return transform.TweenRotation(target, duration);
    }
}